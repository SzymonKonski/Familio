using Application.Common.Exceptions;
using Application.Features.TodoItems.Commands.CreateTodoItem;
using Application.Features.TodoItems.Commands.DeleteTodoItem;
using Application.Security;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class DeleteTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new DeleteTodoItemCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireValidTodoItemGroupId()
    {
        var userId = await RunAsUser1Async();
        await CreateGroupForCurrentUser();

        var command = new DeleteTodoItemCommand
        {
            Id = 99,
            GroupId = ""
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    public async Task ShouldDeleteTodoItem()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            Content = "Content",
            GroupId = groupId,
            PriorityLevel = PriorityLevel.Low,
            Title = "Title1",
            Role = Role.Parent
        });

        await SendAsync(new DeleteTodoItemCommand
        {
            Id = itemId,
            GroupId = groupId
        });
        var item = await FindAsync<TodoItem>(itemId);
        item.Should().BeNull();
    }
}