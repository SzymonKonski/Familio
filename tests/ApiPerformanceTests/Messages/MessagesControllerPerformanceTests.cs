using System.Text;
using System.Text.Json;
using Application.Features.Messages.Commands.CreateMessage;
using NBomber.Contracts;
using NBomber.CSharp;
using Shouldly;

namespace Api.PerformanceTests.Messages;

public class MessagesControllerPerformanceTests : BaseTestFixture
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
        var groupId = await Testing.CreateGroupForCurrentUser(account);
        account.Groups.Add(groupId);
    }

    private Account account;

    private IClientFactory<HttpClient> httpFactory;

    [Test]
    public void create_message()
    {
        var createMessage = Step.Create("Create message", httpFactory, async context =>
        {
            var json = JsonSerializer.Serialize(new CreateMessageCommand
                {Content = "Some content", GroupId = account.Groups[0]});
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await context.Client.PostAsync("/api/Messages/Create", data, context.CancellationToken);

            return !response.IsSuccessStatusCode ? Response.Fail(statusCode: (int) response.StatusCode) : Response.Ok();
        });

        const int expectedRequestsPerSecond = 100;
        const int durationSeconds = 5;

        var scenario = ScenarioBuilder.CreateScenario("Create message", createMessage)
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(Simulation.KeepConstant(50, TimeSpan.FromSeconds(5)));

        var stats = NBomberRunner.RegisterScenarios(scenario).Run();

        TestContext.WriteLine($"OK: {stats.OkCount}, FAILED: {stats.FailCount}");

        stats.OkCount.ShouldBeGreaterThanOrEqualTo(durationSeconds * expectedRequestsPerSecond);
    }

    [Test]
    public void get_messages()
    {
        var getMessages = Step.Create("Get messages", httpFactory, async context =>
        {
            var response = await context.Client.GetAsync($"/api/Messages/GetMessages?GroupId={account.Groups[0]}",
                context.CancellationToken);

            return !response.IsSuccessStatusCode ? Response.Fail(statusCode: (int) response.StatusCode) : Response.Ok();
        });

        const int expectedRequestsPerSecond = 100;
        const int durationSeconds = 5;

        var scenario = ScenarioBuilder.CreateScenario("Get messages", getMessages)
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(Simulation.KeepConstant(50, TimeSpan.FromSeconds(5)));

        var stats = NBomberRunner.RegisterScenarios(scenario).Run();

        TestContext.WriteLine($"OK: {stats.OkCount}, FAILED: {stats.FailCount}");

        stats.OkCount.ShouldBeGreaterThanOrEqualTo(durationSeconds * expectedRequestsPerSecond);
    }
}