using FluentValidation;

namespace Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(v => v.Firstname).NotEmpty();
        RuleFor(v => v.Surname).NotEmpty();
    }
}