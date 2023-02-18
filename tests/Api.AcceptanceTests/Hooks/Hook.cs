using BoDi;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.AcceptanceTests.Hooks;

[Binding]
public class Hook
{
    private const string BaseUri = "https://localhost:5001";
    private readonly IObjectContainer _objectContainer;

    public Hook(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }

    [BeforeScenario]
    public async Task RegisterServices()
    {
        var factory = GetWebApplicationFactory();
        _objectContainer.RegisterInstanceAs(factory);
    }

    private WebApplicationFactory<Program> GetWebApplicationFactory()
    {
        return new CustomWebApplicationFactory();
    }
}