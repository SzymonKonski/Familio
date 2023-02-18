using Application.Common.Exceptions;
using Application.Features.Groups.Commands.UpdateUsernameInGroup;
using Application.Security;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Groups.Commands;

using static Testing;

public class UpdateUsernameInGroupTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new UpdateUsernameInGroupCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new UpdateUsernameInGroupCommand
        {
            GroupId = groupId
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldUpdateUsernameInGroup()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new UpdateUsernameInGroupCommand
        {
            GroupId = groupId,
            UserId = userId,
            Username = "NewUserName"
        };

        await SendAsync(command);
        var item = await FindAsync<DomainUserGroup>(groupId, userId);
        item.Should().NotBeNull();
        item?.UserName.Should().Be(command.Username);
    }
}