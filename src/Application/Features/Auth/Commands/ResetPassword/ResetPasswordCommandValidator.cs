using FluentValidation;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(v => v.UserId).Length(36).NotEmpty();
        RuleFor(v => v.Password).MinimumLength(6).MaximumLength(100).NotEmpty();
        RuleFor(v => v.ConfirmPassword).Equal(v => v.Password).When(v => !string.IsNullOrWhiteSpace(v.Password));
    }
}