using Application.Common.Exceptions;
using Application.Features.Groups.Commands.SendInvitationToGroup;
using Application.Security;
using Domain.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Groups.Commands;

using static Testing;

public class SendInvitationToGroupTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new SendInvitationToGroupCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new SendInvitationToGroupCommand
        {
            GroupId = groupId
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldSendInvitationToGroup()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();
        var secondUserId = await CreateUser("test@gmail.com", "test", "test", "Test1234!");

        var command = new SendInvitationToGroupCommand
        {
            GroupId = groupId,
            Role = Role.Child,
            Email = "test@gmail.com"
        };

        await SendAsync(command);

        var invitation = await FindInvitationsAsync(secondUserId, groupId);

        invitation.Count.Should().Be(1);
        invitation.First().Completed.Should().Be(false);
        invitation.First().DomainUserId.Should().Be(secondUserId);
    }
}