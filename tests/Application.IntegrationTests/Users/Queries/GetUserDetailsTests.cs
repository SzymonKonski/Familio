using Application.Features.Users.Queries.GetUserDetails;
using Application.Security;
using FluentAssertions;
using NUnit.Framework;
using static Application.IntegrationTests.Testing;

namespace Application.IntegrationTests.Users.Queries;

public class GetUserDetailsTests : BaseTestFixture
{
    [Test]
    public async Task HappyPath_ShouldReturnUserDetails()
    {
        await RunAsUser1Async();
        var query = new GetUserDetailsQuery();
        var result = await SendAsync(query);
        result.Email.Should().NotBeEmpty();
    }

    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new GetUserDetailsQuery();
        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();
        var action = () => SendAsync(query);
        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}