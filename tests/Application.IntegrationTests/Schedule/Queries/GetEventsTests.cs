using Application.Features.Schedule.Queries.GetEvents;
using Application.Security;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Schedule.Queries;

using static Testing;

public class GetEventsTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new GetEventsQuery();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldReturnEvents()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();
        var domainGroup = await FindAsync<DomainUserGroup>(groupId, userId);

        await AddAsync(new Event
        {
            Description = "Desc1",
            EventStart = DateTime.Now,
            EventEnd = DateTime.Now.AddDays(2),
            GroupId = groupId,
            CreatedByUser = null,
            UserId = userId
        });

        await AddAsync(new Event
        {
            Description = "Desc2",
            EventStart = DateTime.Now.AddDays(1),
            EventEnd = DateTime.Now.AddDays(3),
            GroupId = groupId,
            CreatedByUser = null,
            UserId = userId
        });


        var query = new GetEventsQuery
        {
            GroupId = groupId
        };

        var result = await SendAsync(query);

        result.Count.Should().Be(2);
        result.First().Description.Should().Be("Desc1");
    }
}