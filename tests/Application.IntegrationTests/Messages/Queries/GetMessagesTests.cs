using Application.Features.Messages.Queries.GetMessages;
using Application.Security;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Messages.Queries;

using static Testing;

public class GetMessagesTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new GetMessagesQuery();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldReturnMessages()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();
        var domainGroup = await FindAsync<DomainUserGroup>(groupId, userId);

        await AddAsync(new Message
        {
            GroupId = groupId,
            CreatedByUser = null,
            UserId = userId,
            Timestamp = DateTime.Now,
            Content = "Content1"
        });

        await AddAsync(new Message
        {
            GroupId = groupId,
            CreatedByUser = null,
            UserId = userId,
            Timestamp = DateTime.Now,
            Content = "Content2"
        });

        var query = new GetMessagesQuery
        {
            GroupId = groupId,
            MessageId = null
        };

        var result = await SendAsync(query);

        result.Count.Should().Be(2);
        result.First().Content.Should().Be("Content2");
    }
}