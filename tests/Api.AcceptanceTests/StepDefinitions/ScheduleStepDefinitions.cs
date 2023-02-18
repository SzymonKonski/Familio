using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Api.AcceptanceTests.StepDefinitions;

[Binding]
public class ScheduleStepDefinitions
{
    private const string BaseAddress = "http://localhost/";

    public ScheduleStepDefinitions(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
    }

    public WebApplicationFactory<Program> Factory { get; }
    public HttpClient Client { get; set; } = null!;

    [Given(@"I am a user")]
    public void GivenIAmAUser()
    {
        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost/5001") // The base address of the test server is http://localhost
        });
    }

    [Given(@"I am in some group")]
    public async void GivenIAmInSomeGroup()
    {
        var response = await Client.GetAsync("/api/Messages/GetMessages?GroupId=1234");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [When(@"I make a GET request to GetEvents endpoint")]
    public void WhenIMakeAGETRequestToGetEventsEndpoint()
    {
        var t = 12;
    }

    [Then(@"the response status code is '([^']*)'")]
    public void ThenTheResponseStatusCodeIs(string p0)
    {
        var t = 12;
    }

    [Then(@"the response should be list of Events in the group")]
    public void ThenTheResponseShouldBeListOfEventsInTheGroup()
    {
        var t = 12;
    }
}