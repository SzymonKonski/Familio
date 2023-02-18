using Application.Common.Exceptions;
using Application.Features.Messages.Commands.CreateMessage;
using Application.Security;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Messages.Commands;

using static Testing;

public class CreateMessageTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new CreateMessageCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new CreateMessageCommand
        {
            GroupId = groupId
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateMessage()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new CreateMessageCommand
        {
            GroupId = groupId,
            Content = "Content"
        };

        var messageId = await SendAsync(command);
        var message = await FindAsync<Message>(messageId);

        message.Should().NotBeNull();
        message!.Content.Should().Be(command.Content);
    }
}