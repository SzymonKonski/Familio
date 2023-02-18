using Application.Features.TodoItems.Commands.CreateTodoItem;
using Application.Features.TodoItems.Commands.SetTodoItemAsDone;
using Application.Security;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class SetTodoItemAsDoneTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new CreateTodoItemCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldSetTodoItemAsDone()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var todoItemId = await SendAsync(new CreateTodoItemCommand
        {
            Content = "Content",
            GroupId = groupId,
            PriorityLevel = PriorityLevel.Low,
            Title = "Title",
            Role = Role.Parent
        });

        var command = new SetTodoItemAsDoneCommand
        {
            Id = todoItemId,
            Done = true
        };
        await SendAsync(command);

        var item = await FindAsync<TodoItem>(todoItemId);

        item.Should().NotBeNull();
        item!.Done.Should().Be(command.Done);
        item.LastModifiedBy.Should().NotBeNull();
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().NotBeNull();
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}