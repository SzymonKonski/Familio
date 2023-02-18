using Application.Common.Exceptions;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Respawn;
using Respawn.Graph;

namespace Application.IntegrationTests;

[SetUpFixture]
public class Testing
{
    private static WebApplicationFactory<Program> _factory = null!;
    private static IConfiguration _configuration = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static Checkpoint _checkpoint = null!;
    private static string? _currentUserId;

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

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static string? GetCurrentUserId()
    {
        return _currentUserId;
    }

    public static async Task<string> RunAsUser1Async()
    {
        return await RunAsUserAsync("pkwola@gmail.com", "Szymon", "Konski", "Test1234!");
    }

    public static async Task CreateTodoItem(string userId, string groupId, string content, string title,
        PriorityLevel level, Role role, bool done)
    {
        using var scope = _scopeFactory.CreateScope();

        var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var domainGroup =
            await _context.DomainUserGroups.FirstOrDefaultAsync(x =>
                x.DomainUserId == userId && x.GroupId == groupId);
        if (domainGroup == null) throw new NotFoundException(nameof(DomainUserGroup));

        var entity = new TodoItem
        {
            Content = content,
            GroupId = domainGroup.GroupId,
            Priority = level,
            Title = title,
            Done = done,
            CreatedByUser = domainGroup,
            UserId = domainGroup.DomainUserId,
            AssignedUserId = null,
            AllowedRole = role.ToString()
        };

        _context.TodoItems.Add(entity);
        await _context.SaveChangesAsync();
    }

    public static async Task<string> CreateGroupForCurrentUser()
    {
        if (_currentUserId == null) await RunAsUser1Async();

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
        var user1 = await context.Users.FirstOrDefaultAsync(u => u.Id == _currentUserId);

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

    public static async Task<string> RunAsUserAsync(string email, string firstName, string surName, string password)
    {
        using var scope = _scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<DomainUser>>();

        var user = new DomainUser
        {
            Email = email,
            Firstname = firstName,
            Surname = surName,
            UserName = email,
            DomainUsername = firstName + ' ' + surName
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            _currentUserId = user.Id;
            return _currentUserId;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Unable to create {email}.{Environment.NewLine}{errors}");
    }

    public static async Task<string?> CreateUser(string email, string firstName, string surName, string password)
    {
        using var scope = _scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<DomainUser>>();

        var user = new DomainUser
        {
            Email = email,
            Firstname = firstName,
            Surname = surName,
            UserName = email,
            DomainUsername = firstName + ' ' + surName
        };

        var result = await userManager.CreateAsync(user, password);
        var code1 = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await userManager.ConfirmEmailAsync(user, code1);

        return user.Id;
    }

    public static async Task ResetState()
    {
        var connection = _configuration.GetConnectionString("DefaultConnection");
        await _checkpoint.Reset(connection);
        _currentUserId = null;
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task<List<InvitationToGroup>> FindInvitationsAsync(string userId, string groupId)
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Invitations.Where(x => x.DomainUserId == userId && x.GroupId == groupId).ToListAsync();
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
    }
}