using FluentValidation;

namespace Application.Features.Auth.Commands.CreateToken;

public class CreateTokenCommandValidator : AbstractValidator<CreateTokenCommand>
{
    public CreateTokenCommandValidator()
    {
        RuleFor(v => v.Email).EmailAddress();
        RuleFor(v => v.Password);
    }
}