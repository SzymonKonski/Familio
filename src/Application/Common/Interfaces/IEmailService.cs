namespace Application.Common.Interfaces;

public interface IEmailService
{
    Task<bool> SendAsync(string emailDisplayName, string subject, string body, string from, string to);

    Task<bool> SendEmailConfirmationAsync(string email, string mobileCode, string emailCode);

    Task<bool> SendPasswordResetAsync(string email, string mobileCode, string emailCode);

    Task<bool> SendInvitationLinkAsync(string emailAddress, string mobileCode, string emailCode, string groupName);
}