using Application.Common.Exceptions;
using Application.Features.Users.Commands.UploadAvatarImage;
using Application.Security;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Users.Commands;

using static Testing;

public class UploadAvatarImageTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new UploadAvatarImageCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var userId = await RunAsUser1Async();

        var command = new UploadAvatarImageCommand();

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }
}