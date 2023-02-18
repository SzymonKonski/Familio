using System.Net;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Services.AppSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class EmailCallbackService : IEmailCallbackService
{
    private readonly ClientAppSettings _client;
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IMobileCodeService _mobileCodeService;
    private readonly UserManager<DomainUser> _userManager;

    public EmailCallbackService(
        UserManager<DomainUser> userManager,
        IEmailService emailService, IOptions<ClientAppSettings> clientAppSettings, IApplicationDbContext context,
        IMobileCodeService mobileCodeService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _context = context;
        _mobileCodeService = mobileCodeService;
        _client = clientAppSettings.Value;
    }

    public async Task<bool> SendConfirmationEmail(DomainUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var mobileCode = await _mobileCodeService.GenerateCode(user.Id, AuthActionType.ConfirmEmail, token);
        var emailCode =
            $"{_client.Url}/{_client.EmailConfirmationPath}?uid={user.Id}&code={WebUtility.UrlEncode(mobileCode)}";

        return await _emailService.SendEmailConfirmationAsync(user.Email, mobileCode, emailCode);
    }

    public async Task<bool> SendInvitationLink(DomainUser user, Group group)
    {
        var token = await _userManager.GenerateUserTokenAsync(user, "CustomTokenProvider", group.Id);
        var mobileCode = await _mobileCodeService.GenerateCode(user.Id, AuthActionType.SendInvitation, token);
        var emailCode =
            $"{_client.Url}/{_client.AddUserToGroupPath}?uid={user.Id}&code={WebUtility.UrlEncode(mobileCode)}";

        return await _emailService.SendInvitationLinkAsync(user.Email, mobileCode, emailCode, group.Name);
    }

    public async Task<bool> SendResetPasswordLink(DomainUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var mobileCode = await _mobileCodeService.GenerateCode(user.Id, AuthActionType.ResetPassword, token);
        var emailCode =
            $"{_client.Url}/{_client.ResetPasswordPath}?uid={user.Id}&code={WebUtility.UrlEncode(mobileCode)}";

        return await _emailService.SendPasswordResetAsync(user.Email, mobileCode, emailCode);
    }
}