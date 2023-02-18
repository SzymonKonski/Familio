using System.Net;
using NUnit.Framework;

namespace Api.AcceptanceTests.StepDefinitions;

[Binding]
[TestFixture]
public class UsersStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;

    public UsersStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"the query is correct")]
    public async Task TheQueryIsCorrect()
    {
        //var response = await _httpClient.GetAsync("/api/Users/GetUserGroups");
        //_scenarioContext.Add("Response", response);
    }

    [When(@"Called GetUserDetails")]
    public void CalledGetUserDetails()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("Response");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Then(@"the user details should be returned")]
    public void TheUserDetailsShouldBeReturned()
    {
    }
}