using FluentValidation;

namespace Application.Features.Groups.Commands.AddUserToGroup;

public class AddUserToGroupCommandValidator : AbstractValidator<AddUserToGroupCommand>
{
    public AddUserToGroupCommandValidator()
    {
        RuleFor(v => v.Code).NotEmpty();
        RuleFor(v => v.UserId).NotEmpty();
    }
}