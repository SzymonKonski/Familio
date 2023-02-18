using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Infrastructure.UnitTests.Services;

public class IdentityServiceTests
{
    private Mock<IApplicationDbContext> _applicationDbContext = null!;
    private Mock<IAuthorizationService> _authorizationService = null!;
    private Mock<IUserClaimsPrincipalFactory<DomainUser>> _userClaimsPrincipalFactory = null!;
    private Mock<UserManager<DomainUser>> _userManager = null!;

    [SetUp]
    public void Setup()
    {
        _userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<DomainUser>>();
        _applicationDbContext = new Mock<IApplicationDbContext>();
        _userManager = new Mock<UserManager<DomainUser>>(Mock.Of<IUserStore<DomainUser>>(), null, null, null,
            null, null,
            null, null, null);
        _authorizationService = new Mock<IAuthorizationService>();
    }

    [Test]
    public Task IsInRoleAsync_ShouldThrowNotFoundException_WhenUserIsNotFound()
    {
        var userId = "123";
        var groupId = "123";
        var role = "Parent";

        _userManager.Setup(x => x.FindByIdAsync(userId))!.ReturnsAsync((DomainUser) null);

        var identityService = new IdentityService(_userManager.Object, _userClaimsPrincipalFactory.Object,
            _authorizationService.Object, _applicationDbContext.Object);

        Assert.ThrowsAsync<NotFoundException>(() => identityService.IsInRoleAsync(userId, groupId, role));
        return Task.CompletedTask;
    }

    [Test]
    public async Task IsInRoleAsync_ShouldReturnFalse_WhenUserIsRemoved()
    {
        var userId = "123";
        var groupId = "123";
        var role = "Parent";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("MovieListDatabase")
            .Options;

        await using (var context = new ApplicationDbContext(options, null))
        {
            context.DomainUserGroups.Add(new DomainUserGroup
                {GroupId = groupId, DomainUserId = userId, Role = role, UserRemoved = true, UserName = "UserName"});
            await context.SaveChangesAsync();
        }

        var user = new DomainUser();

        _userManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

        await using (var context = new ApplicationDbContext(options, null))
        {
            var identityService = new IdentityService(_userManager.Object, _userClaimsPrincipalFactory.Object,
                _authorizationService.Object, context);

            var result = await identityService.IsInRoleAsync(userId, role, groupId);

            result.ShouldBe(false);
        }
    }
}