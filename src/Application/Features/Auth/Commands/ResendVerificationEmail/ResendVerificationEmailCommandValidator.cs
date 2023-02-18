using FluentValidation;

namespace Application.Features.Auth.Commands.ResendVerificationEmail;

public class ResendVerificationEmailCommandValidator : AbstractValidator<ResendVerificationEmailCommand>
{
    public ResendVerificationEmailCommandValidator()
    {
        RuleFor(v => v.Email)
            .EmailAddress()
            .NotEmpty();
    }
}