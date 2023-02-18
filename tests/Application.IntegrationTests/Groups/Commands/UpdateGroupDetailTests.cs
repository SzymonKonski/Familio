using Application.Common.Exceptions;
using Application.Features.Groups.Commands.CreateGroup;
using Application.Features.Groups.Commands.UpdateGroupDetail;
using Application.Security;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Groups.Commands;

using static Testing;

public class UpdateGroupDetailTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new UpdateGroupDetailCommand();

        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var userId = await RunAsUser1Async();
        var groupId = await CreateGroupForCurrentUser();

        var command = new UpdateGroupDetailCommand
        {
            Name = "",
            GroupId = groupId
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldUpdateGroupDetail()
    {
        var userId = await RunAsUser1Async();

        var groupId = await SendAsync(new CreateGroupCommand
        {
            Name = "GroupName"
        });

        var command = new UpdateGroupDetailCommand
        {
            Name = "NewGroupName",
            GroupId = groupId
        };

        await SendAsync(command);

        var item = await FindAsync<Group>(groupId);
        item.Should().NotBeNull();
        item.Name.Should().Be(command.Name);
        item.CreatedBy.Should().Be(userId);
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}