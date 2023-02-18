using System.Text;
using Application.Features.Users.Commands.UpdateUserProfile;
using NBomber.Contracts;
using NBomber.CSharp;
using Newtonsoft.Json;
using Shouldly;

namespace Api.PerformanceTests.Users;

public class UsersControllerPerformanceTests : BaseTestFixture
{
    [SetUp]
    public async Task TestSetUp()
    {
        account = TestAccounts.TestAccount1();
        var userId = await Testing.CreateUserAsync(account);
        account.UserId = userId;
        var token = await Testing.GetUserToken(account);
        account.Token = token;
        httpFactory = await Testing.GetClientFactory(account.Token.AccessToken);
    }

    private IClientFactory<HttpClient> httpFactory;
    private Account account;

    [Test]
    public void get_user_groups()
    {
        var getUserGroups = Step.Create("Get user groups", httpFactory, async context =>
        {
            var response = await context.Client.GetAsync("/api/Users/GetUserGroups", context.CancellationToken);

            return !response.IsSuccessStatusCode ? Response.Fail(statusCode: (int) response.StatusCode) : Response.Ok();
        });

        const int expectedRequestsPerSecond = 100;
        const int durationSeconds = 5;

        var scenario = ScenarioBuilder.CreateScenario("Fetch user group", getUserGroups)
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(Simulation.KeepConstant(50, TimeSpan.FromSeconds(5)));

        var stats = NBomberRunner.RegisterScenarios(scenario).Run();

        TestContext.WriteLine($"OK: {stats.OkCount}, FAILED: {stats.FailCount}");

        stats.OkCount.ShouldBeGreaterThanOrEqualTo(durationSeconds * expectedRequestsPerSecond);
    }

    [Test]
    public void get_user_details()
    {
        var getUserDetails = Step.Create("Get user details", httpFactory, async context =>
        {
            var response = await context.Client.GetAsync("/api/Users/GetUserDetails", context.CancellationToken);

            return !response.IsSuccessStatusCode ? Response.Fail(statusCode: (int) response.StatusCode) : Response.Ok();
        });

        const int expectedRequestsPerSecond = 100;
        const int durationSeconds = 5;

        var scenario = ScenarioBuilder.CreateScenario("Fetch user details", getUserDetails)
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(Simulation.KeepConstant(50, TimeSpan.FromSeconds(5)));

        var stats = NBomberRunner.RegisterScenarios(scenario).Run();

        TestContext.WriteLine($"OK: {stats.OkCount}, FAILED: {stats.FailCount}");

        stats.OkCount.ShouldBeGreaterThanOrEqualTo(durationSeconds * expectedRequestsPerSecond);
    }

    [Test]
    public void update_user_profile()
    {
        var request = new UpdateUserProfileCommand
        {
            Surname = "SurnameTest",
            Firstname = "FirstNameTest"
        };
        var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var updateUserProfile = Step.Create("Update user profile", httpFactory, async context =>
        {
            var response =
                await context.Client.PostAsync("/api/Users/UpdateUserProfile", stringContent,
                    context.CancellationToken);

            return !response.IsSuccessStatusCode ? Response.Fail(statusCode: (int) response.StatusCode) : Response.Ok();
        });

        const int expectedRequestsPerSecond = 100;
        const int durationSeconds = 5;

        var scenario = ScenarioBuilder.CreateScenario("Fetch user details", updateUserProfile)
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(Simulation.KeepConstant(2, TimeSpan.FromSeconds(5)));

        var stats = NBomberRunner.RegisterScenarios(scenario).Run();

        TestContext.WriteLine($"OK: {stats.OkCount}, FAILED: {stats.FailCount}");

        stats.OkCount.ShouldBeGreaterThanOrEqualTo(durationSeconds * expectedRequestsPerSecond);
    }
}