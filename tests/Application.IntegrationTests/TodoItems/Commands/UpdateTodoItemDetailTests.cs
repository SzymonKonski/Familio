using Application.Common.Exceptions;
using Application.Features.TodoItems.Commands.CreateTodoItem;
using Application.Features.TodoItems.Commands.UpdateTodoItemDetail;
using Application.Security;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class UpdateTodoItemDetailTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new UpdateTodoItemDetailCommand();
        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();
        var action = () => SendAsync(query);
        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldThrowNotFoundExceptionWhenMissingTodoItem()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new UpdateTodoItemDetailCommand
        {
            Id = 99,
            Priority = PriorityLevel.High,
            Note = "NewContent",
            Title = "NewTitle"
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task
        ShouldThrowUnauthorizedAccessException_WhenUserUpdateTodoItem_ThatHeIsNotAssignedTo_And_HeIsNotParent()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();
        var domainGroup = await FindAsync<DomainUserGroup>(groupId, userId);

        var todoItemId = await SendAsync(new CreateTodoItemCommand
        {
            Content = "Content",
            GroupId = groupId,
            PriorityLevel = PriorityLevel.Low,
            Title = "Title",
            Role = Role.Parent
        });

        var command = new UpdateTodoItemDetailCommand
        {
            Id = todoItemId,
            Priority = PriorityLevel.High,
            Note = "NewContent",
            Title = "NewTitle"
        };
    }

    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var userId = await RunAsUser1Async();
        var command = new UpdateTodoItemDetailCommand();
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task HappyPath_ShouldUpdateTodoItemDetail()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();
        var domainGroup = await FindAsync<DomainUserGroup>(groupId, userId);

        var todoItemId = await SendAsync(new CreateTodoItemCommand
        {
            Content = "Content",
            GroupId = groupId,
            PriorityLevel = PriorityLevel.Low,
            Title = "Title",
            Role = Role.Parent
        });

        var command = new UpdateTodoItemDetailCommand
        {
            Id = todoItemId,
            Priority = PriorityLevel.High,
            Note = "NewContent",
            Title = "NewTitle"
        };
        await SendAsync(command);

        var todoItem = await FindAsync<TodoItem>(todoItemId);

        todoItem.Should().NotBeNull();
        todoItem!.Title.Should().Be(command.Title);
        todoItem.LastModifiedBy.Should().NotBeNull();
        todoItem.LastModifiedBy.Should().Be(userId);
        todoItem.LastModified.Should().NotBeNull();
        todoItem.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}