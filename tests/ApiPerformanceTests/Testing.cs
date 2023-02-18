using System.Net.Http.Headers;
using System.Text;
using Domain.Entities;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NBomber.Contracts;
using NBomber.CSharp;
using Newtonsoft.Json;
using Respawn;
using Respawn.Graph;

namespace Api.PerformanceTests;

public class LoginData
{
    public string? Email { get; set; }

    public string? Password { get; set; }
}

public class TokenModel
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}

[SetUpFixture]
public class Testing
{
    private const string BaseUri = "https://localhost:5001";
    private const string loginUrl = "https://localhost:5001/api/Auth/CreateToken";
    private static WebApplicationFactory<Program> _factory = null!;
    private static IConfiguration _configuration = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static Checkpoint _checkpoint = null!;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        _factory = new CustomWebApplicationFactory();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();

        _checkpoint = new Checkpoint
        {
            TablesToIgnore = new Table[] {"__EFMigrationsHistory"}
        };
    }

    public static async Task ResetState()
    {
        var connection = _configuration.GetConnectionString("DefaultConnection");
        await _checkpoint.Reset(connection);
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
    }

    public static async Task<IClientFactory<HttpClient>> GetClientFactory(string accessToken)
    {
        return ClientFactory.Create(
            "http_factory",
            clientCount: 1,
            initClient: async (number, _) =>
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                return client;
            });
    }

    public static async Task<string> CreateUserAsync(Account account)
    {
        using var scope = _scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<DomainUser>>();

        var user = new DomainUser
        {
            Email = account.Email,
            Firstname = account.Firstname,
            Surname = account.Surname,
            UserName = account.Email,
            DomainUsername = account.Firstname + ' ' + account.Surname
        };

        var result = await userManager.CreateAsync(user, account.Password);
        var code1 = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await userManager.ConfirmEmailAsync(user, code1);
        if (result.Succeeded) return user.Id;

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Unable to create {account.Email}.{Environment.NewLine}{errors}");
    }

    public static async Task<TokenModel> GetUserToken(Account account)
    {
        var token = new TokenModel();

        var loginData = new LoginData
        {
            Email = account.Email,
            Password = account.Password
        };
        var json = JsonConvert.SerializeObject(loginData);

        using var client = new HttpClient();
        client.BaseAddress = new Uri(BaseUri);

        // call your authentication server
        var response = await client.PostAsync(loginUrl, new StringContent(json, Encoding.UTF8, "application/json"));
        var content = await response.Content.ReadAsStringAsync();

        try
        {
            token = JsonConvert.DeserializeObject<TokenModel>(content);
        }
        catch (JsonReaderException)
        {
            Console.WriteLine("Invalid JSON.");
        }

        return token;
    }

    public static async Task<string> CreateGroupForCurrentUser(Account account)
    {
        if (account.UserId == null) await CreateUserAsync(account);

        var groupId1 = Guid.NewGuid().ToString();

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Groups.Add(new Group
        {
            Id = groupId1,
            Name = "Group1"
        });

        await context.SaveChangesAsync();

        var group1 = await context.Groups.FirstOrDefaultAsync(x => x.Id == groupId1);
        var user1 = await context.Users.FirstOrDefaultAsync(u => u.Id == account.UserId);

        context.DomainUserGroups.Add(new DomainUserGroup
        {
            DomainUser = user1,
            DomainUserId = user1.Id,
            Group = group1,
            GroupId = group1.Id,
            Role = "Parent",
            UserName = "pkwola"
        });

        await context.SaveChangesAsync();

        return groupId1;
    }
}