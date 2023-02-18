using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Services;
using Infrastructure.Services.AppSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Shouldly;

namespace Infrastructure.UnitTests.Services;

public class TokenServiceTests
{
    private Mock<IApplicationDbContext> _applicationDbContext = null!;
    private IOptions<JwtSecurityTokenSettings> _jwt = null!;
    private Mock<UserManager<DomainUser>> _userManager = null!;
    private TokenValidationParameters _validationParameters = null!;

    [SetUp]
    public void Setup()
    {
        _jwt = Options.Create(new JwtSecurityTokenSettings());
        _applicationDbContext = new Mock<IApplicationDbContext>();
        _userManager = new Mock<UserManager<DomainUser>>(Mock.Of<IUserStore<DomainUser>>(), null, null, null,
            null, null,
            null, null, null);
        _validationParameters = new TokenValidationParameters();
    }

    [Test]
    public async Task CreateTokenForVerifiedUser_ShouldReturnFalse_WhenUserDoesNotExists()
    {
        // Act
        var tokenService =
            new TokenService(_jwt, _validationParameters, _userManager.Object, _applicationDbContext.Object);

        // Assert
        var result = await tokenService.CreateTokenForVerifiedUser("test", "test", CancellationToken.None);
        result.Success.ShouldBe(false);
        result.Errors.ShouldContain("Invalid credentials.");
    }

    [Test]
    public async Task CreateTokenForVerifiedUser_ShouldReturnFalse_WhenUserEmailIsNotConfirmed()
    {
        // Act
        var tokenService =
            new TokenService(_jwt, _validationParameters, _userManager.Object, _applicationDbContext.Object);
        _userManager.Setup(x => x.FindByEmailAsync("test@gmail.com"))
            .ReturnsAsync(new DomainUser {EmailConfirmed = false});

        // Assert
        var result = await tokenService.CreateTokenForVerifiedUser("test@gmail.com", "test", CancellationToken.None);
        result.Success.ShouldBe(false);
        result.Errors.ShouldContain("Invalid credentials.");
    }

    [Test]
    public async Task CreateTokenForVerifiedUser_ShouldReturnFalse_WhenUserIsLocked()
    {
        // Act
        var tokenService =
            new TokenService(_jwt, _validationParameters, _userManager.Object, _applicationDbContext.Object);
        _userManager.Setup(x => x.FindByEmailAsync("test@gmail.com"))
            .ReturnsAsync(new DomainUser {EmailConfirmed = true, LockoutEnabled = true});

        // Assert
        var result = await tokenService.CreateTokenForVerifiedUser("test@gmail.com", "test", CancellationToken.None);
        result.Success.ShouldBe(false);
        result.Errors.ShouldContain("This account has been locked.");
    }

    [Test]
    public async Task CreateTokenForVerifiedUser_ShouldReturnFalse_WhenUserPasswordIsWrong()
    {
        // Act
        var tokenService =
            new TokenService(_jwt, _validationParameters, _userManager.Object, _applicationDbContext.Object);

        var user = new DomainUser {EmailConfirmed = true, LockoutEnabled = false};

        _userManager.Setup(x => x.FindByEmailAsync("test@gmail.com"))
            .ReturnsAsync(user);
        _userManager.Setup(x => x.CheckPasswordAsync(user, ""))
            .ReturnsAsync(false);

        // Assert
        var result = await tokenService.CreateTokenForVerifiedUser("test@gmail.com", "test", CancellationToken.None);
        result.Success.ShouldBe(false);
        result.Errors.ShouldContain("Invalid login attempt.");
    }
}