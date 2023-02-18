using FluentValidation;

namespace Application.Features.Groups.Commands.CreateGroup;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(v => v.Name).MaximumLength(200).NotEmpty();
    }
}