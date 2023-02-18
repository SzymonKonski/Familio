using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Infrastructure.Services.AppSettings;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _email;
    private readonly string _pathToEmailTemplates;

    public EmailService(IOptions<EmailSettings> email, IPathProvider pathProvider)
    {
        _email = email.Value;
        _pathToEmailTemplates = pathProvider.MapPath(Path.Combine("EmailTemplates", "html"));
    }

    public async Task<bool> SendAsync(string emailDisplayName, string subject, string body, string from, string to)
    {
        return await SendSendGridMessage(from, emailDisplayName, new List<EmailAddress> {new(to)}, subject, body);
    }

    public async Task<bool> SendEmailConfirmationAsync(string email, string mobileCode, string emailCode)
    {
        if (_email.From == null || _email.DisplayName == null)
            throw new NotFoundException("Emails Settings not found");

        var path = Path.Combine(_pathToEmailTemplates, "ConfirmEmail.html");
        var htmlTemplate = await File.ReadAllTextAsync(path);
        var htmlContent = htmlTemplate.Replace("%%confirm_email_code%%", mobileCode)
            .Replace("%%confirm_email_link%%", emailCode);
        var subject = "Confirm your email";

        return await SendSendGridMessage(_email.From, _email.DisplayName,
            new List<EmailAddress> {new(email)}, subject, htmlContent);
    }

    public async Task<bool> SendInvitationLinkAsync(string emailAddress, string mobileCode, string emailCode,
        string groupName)
    {
        if (_email.From == null || _email.DisplayName == null)
            throw new NotFoundException("Emails Settings not found");

        var path = Path.Combine(_pathToEmailTemplates, "GroupInvite.html");
        var htmlTemplate = await File.ReadAllTextAsync(path);
        var htmlContent = htmlTemplate.Replace("%%accept_invitation_code%%", mobileCode)
            .Replace("%%accept_invitation_link%%", emailCode).Replace("%%group_name%%", groupName);
        var subject = $"Invitation to group {groupName}";

        return await SendSendGridMessage(_email.From, _email.DisplayName,
            new List<EmailAddress> {new(emailAddress)}, subject, htmlContent);
    }

    public async Task<bool> SendPasswordResetAsync(string emailAddress, string mobileCode, string emailCode)
    {
        if (_email.From == null || _email.DisplayName == null)
            throw new NotFoundException("Emails Settings not found");

        var path = Path.Combine(_pathToEmailTemplates, "ResetPassword.html");
        var htmlTemplate = await File.ReadAllTextAsync(path);
        var htmlContent = htmlTemplate.Replace("%%reset_password_code%%", mobileCode)
            .Replace("%%reset_password_link%%", emailCode);
        var subject = "Reset your password";

        return await SendSendGridMessage(_email.From, _email.DisplayName,
            new List<EmailAddress> {new(emailAddress)}, subject, htmlContent);
    }

    private async Task<bool> SendSendGridMessage(string fromEmail, string emailDisplayName, List<EmailAddress> tos,
        string subject,
        string htmlContent)
    {
        var client = new SendGridClient(_email.SendGridApiKey);
        var from = new EmailAddress(fromEmail, emailDisplayName);
        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, "", htmlContent, false);
        var response = await client.SendEmailAsync(msg);

        return response.IsSuccessStatusCode;
    }
}