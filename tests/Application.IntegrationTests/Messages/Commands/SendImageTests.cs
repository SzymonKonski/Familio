using Application.Common.Exceptions;
using Application.Features.Messages.Commands.SendImage;
using Application.Security;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Messages.Commands;

using static Testing;

public class SendImageTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new SendImageCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new SendImageCommand
        {
            GroupId = groupId
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }
}