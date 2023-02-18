using Application.Common.Exceptions;
using Application.Features.Schedule.Commands.CreateEvent;
using Application.Security;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Schedule.Commands;

using static Testing;

public class CreateEventTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new CreateEventCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new CreateEventCommand
        {
            GroupId = groupId
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateEvent()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new CreateEventCommand
        {
            GroupId = groupId,
            Description = "Desc",
            EventStart = DateTime.Now,
            EventEnd = DateTime.Now.AddDays(2)
        };

        var eventId = await SendAsync(command);
        var @event = await FindAsync<Event>(eventId);

        @event.Should().NotBeNull();
        @event!.Description.Should().Be(command.Description);
    }
}