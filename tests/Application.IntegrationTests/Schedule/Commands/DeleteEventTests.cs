using Application.Common.Exceptions;
using Application.Features.Schedule.Commands.CreateEvent;
using Application.Features.Schedule.Commands.DeleteEvent;
using Application.Security;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Schedule.Commands;

using static Testing;

public class DeleteEventTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new DeleteEventCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new DeleteEventCommand
        {
            Id = 99,
            GroupId = groupId
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteEvent()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var itemId = await SendAsync(new CreateEventCommand
        {
            GroupId = groupId,
            EventStart = DateTime.Now,
            EventEnd = DateTime.Now,
            Description = "Desc"
        });

        await SendAsync(new DeleteEventCommand
        {
            Id = itemId,
            GroupId = groupId
        });
        var item = await FindAsync<Event>(itemId);
        item.Should().BeNull();
    }
}