using Application.Common.Exceptions;
using Application.Features.TodoItems.Queries.GetDoneTodoItemsWithPagination;
using Application.Security;
using Domain.Common;
using Domain.Enums;
using FluentAssertions;
using NUnit.Framework;
using static Application.IntegrationTests.Testing;


namespace Application.IntegrationTests.TodoItems.Queries;

public class GetDoneTodoItemsWithPaginationTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new GetDoneTodoItemsWithPaginationQuery();
        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();
        var action = () => SendAsync(query);
        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldThrowNotFoundExceptionWhenMissingGroup()
    {
        var userId = await RunAsUser1Async();
        var query = new GetDoneTodoItemsWithPaginationQuery
        {
            GroupId = "123"
        };
        await FluentActions.Invoking(() =>
            SendAsync(query)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task HappyPath_ShouldReturnDoneTodoItems()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        await CreateTodoItem(userId, groupId, "Content1", "Title1", PriorityLevel.High, Role.Child, true);
        await CreateTodoItem(userId, groupId, "Content2", "Title2", PriorityLevel.High, Role.Child, true);

        var query = new GetDoneTodoItemsWithPaginationQuery
        {
            GroupId = groupId
        };

        var result = await SendAsync(query);

        result.Items.Should().HaveCount(2);
        result.PageNumber.Should().Be(1);
        result.Items.First().Title.Should().Be("Title1");
    }
}