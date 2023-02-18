using FluentValidation;

namespace Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.Email)
            .EmailAddress();
        RuleFor(v => v.Password)
            .MinimumLength(6)
            .MaximumLength(100)
            .NotEmpty();
        RuleFor(v => v.ConfirmPassword)
            .Equal(v => v.Password)
            .When(v => !string.IsNullOrWhiteSpace(v.Password));
        RuleFor(v => v.Firstname)
            .MinimumLength(1)
            .MaximumLength(50)
            .NotEmpty();
        RuleFor(v => v.Surname)
            .MinimumLength(1)
            .MaximumLength(50)
            .NotEmpty();
    }
}