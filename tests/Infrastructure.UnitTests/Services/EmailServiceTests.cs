using Application.Common.Exceptions;
using Infrastructure.Services;
using Infrastructure.Services.AppSettings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;

namespace Infrastructure.UnitTests.Services;

public class EmailServiceTests
{
    [Test]
    public void SendEmailConfirmationShouldThrowNotFoundExceptionWhenEmailSettingsNotFound()
    {
        // Arrange
        var someOptions = Options.Create(new EmailSettings());
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        mockEnvironment
            .Setup(m => m.WebRootPath)
            .Returns("c");
        var provider = new PathProvider(mockEnvironment.Object);

        // Act
        var emailService = new EmailService(someOptions, provider);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(() =>
            emailService.SendEmailConfirmationAsync("emailAddress", "mobileCode", "emailCode"));
    }

    [Test]
    public void SendPasswordResetShouldThrowNotFoundExceptionWhenEmailSettingsNotFound()
    {
        // Arrange
        var someOptions = Options.Create(new EmailSettings());
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        mockEnvironment
            .Setup(m => m.WebRootPath)
            .Returns("WebRootPath");
        var provider = new PathProvider(mockEnvironment.Object);

        // Act
        var emailService = new EmailService(someOptions, provider);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(() =>
            emailService.SendPasswordResetAsync("emailAddress", "mobileCode", "emailCode"));
    }

    [Test]
    public void SendInvitationLinkShouldThrowNotFoundExceptionWhenEmailSettingsNotFound()
    {
        // Arrange
        var someOptions = Options.Create(new EmailSettings());
        var mockEnvironment = new Mock<IWebHostEnvironment>();
        mockEnvironment
            .Setup(m => m.WebRootPath)
            .Returns("WebRootPath");
        var provider = new PathProvider(mockEnvironment.Object);

        // Act
        var emailService = new EmailService(someOptions, provider);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(() =>
            emailService.SendInvitationLinkAsync("emailAddress", "mobileCode", "emailCode", "groupName"));
    }
}