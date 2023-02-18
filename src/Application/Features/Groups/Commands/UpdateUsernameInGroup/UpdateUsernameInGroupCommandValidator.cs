using FluentValidation;

namespace Application.Features.Groups.Commands.UpdateUsernameInGroup;

public class UpdateUsernameInGroupCommandValidator : AbstractValidator<UpdateUsernameInGroupCommand>
{
    public UpdateUsernameInGroupCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().Length(36);
        RuleFor(v => v.GroupId).NotEmpty().Length(36);
        RuleFor(v => v.Username).MaximumLength(200).NotEmpty();
    }
}