using Application.Common.Exceptions;
using Application.Features.Users.Commands.UpdateUserProfile;
using Application.Security;
using Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Users.Commands;

using static Testing;

public class UpdateUserProfileTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new UpdateUserProfileCommand();
        query.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();
        var action = () => SendAsync(query);
        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var userId = await RunAsUser1Async();
        var command = new UpdateUserProfileCommand();
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task HappyPath_ShouldUpdateUserProfile()
    {
        var userId = await RunAsUser1Async();

        var command = new UpdateUserProfileCommand
        {
            Firstname = "Simon",
            Surname = "Konski"
        };

        await SendAsync(command);
        var item = await FindAsync<DomainUser>(userId);
        item.Should().NotBeNull();
        item?.Firstname.Should().Be(command.Firstname);
        item?.Surname.Should().Be(command.Surname);
    }
}