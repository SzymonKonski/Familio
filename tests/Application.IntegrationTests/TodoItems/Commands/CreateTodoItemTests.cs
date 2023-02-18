using Application.Common.Exceptions;
using Application.Features.TodoItems.Commands.CreateTodoItem;
using Application.Security;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class CreateTodoItemTests : BaseTestFixture
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
    public async Task ShouldRequireMinimumFields()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new CreateTodoItemCommand
        {
            GroupId = groupId
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateTodoItem()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new CreateTodoItemCommand
        {
            Content = "Content",
            GroupId = groupId,
            PriorityLevel = PriorityLevel.Low,
            Title = "Title1",
            Role = Role.Parent
        };

        var todoItemId = await SendAsync(command);
        var item = await FindAsync<TodoItem>(todoItemId);

        item.Should().NotBeNull();
        item!.Content.Should().Be(command.Content);
        item.Title.Should().Be(command.Title);
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}