using FluentValidation;

namespace Application.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(v => v.Code).NotEmpty();
    }
}