using FluentValidation;

namespace Application.Features.Groups.Commands.RemoveUserFromGroup;

public class DeleteUserFromGroupCommandValidator : AbstractValidator<DeleteUserFromGroupCommand>
{
    public DeleteUserFromGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty().Length(36);
        RuleFor(x => x.UserId).NotEmpty().Length(36);
    }
}