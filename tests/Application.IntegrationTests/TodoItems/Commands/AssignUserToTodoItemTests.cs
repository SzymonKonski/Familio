using Application.Common.Exceptions;
using Application.Features.TodoItems.Commands.AssignUserToTodoItemCommand;
using Application.Features.TodoItems.Commands.CreateTodoItem;
using Application.Security;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class AssignUserToTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new AssignUserToTodoItemCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireGroupId()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new AssignUserToTodoItemCommand();

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ForbiddenAccessException>().WithMessage("GroupId cannot be empty");
    }

    [Test]
    public async Task ShouldAssignUserToTodoItem()
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

        await SendAsync(new AssignUserToTodoItemCommand
        {
            GroupId = groupId,
            UserId = userId,
            TodoItemId = todoItemId
        });

        var todoItem = await FindAsync<TodoItem>(todoItemId);

        todoItem.Should().NotBeNull();
        todoItem!.AssignedUserId.Should().NotBeNull();
        todoItem.AssignedUserId.Should().Be(userId);
        todoItem.LastModifiedBy.Should().NotBeNull();
        todoItem.LastModifiedBy.Should().Be(userId);
        todoItem.LastModified.Should().NotBeNull();
        todoItem.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}