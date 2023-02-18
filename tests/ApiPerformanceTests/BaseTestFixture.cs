namespace Api.PerformanceTests;

[TestFixture]
public abstract class BaseTestFixture
{
    [SetUp]
    public async Task TestSetUp()
    {
        await Testing.ResetState();
    }
}