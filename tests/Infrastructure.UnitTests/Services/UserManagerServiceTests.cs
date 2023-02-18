using Application.Common.Exceptions;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Infrastructure.UnitTests.Services;

public class UserManagerServiceTests
{
    [Test]
    public void ShouldThrowNotFoundExceptionWhenUserNotFound()
    {
        // Arrange
        var userManager = new Mock<UserManager<DomainUser>>(Mock.Of<IUserStore<DomainUser>>(), null, null, null,
            null, null,
            null, null, null);

        // Act
        var userManagerService = new UserManagerService(userManager.Object);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(() => userManagerService.GetUserById("1234"));
    }
}