using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IEmailCallbackService
{
    Task<bool> SendConfirmationEmail(DomainUser user);

    Task<bool> SendInvitationLink(DomainUser user, Group group);

    Task<bool> SendResetPasswordLink(DomainUser user);
}