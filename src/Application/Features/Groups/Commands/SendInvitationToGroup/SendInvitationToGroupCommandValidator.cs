using FluentValidation;

namespace Application.Features.Groups.Commands.SendInvitationToGroup;

public class SendInvitationToGroupCommandValidator : AbstractValidator<SendInvitationToGroupCommand>
{
    public SendInvitationToGroupCommandValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress();

        RuleFor(v => v.GroupId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Role).IsInEnum();
    }
}