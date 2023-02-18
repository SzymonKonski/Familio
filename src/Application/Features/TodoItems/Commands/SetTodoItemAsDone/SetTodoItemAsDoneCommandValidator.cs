using FluentValidation;

namespace Application.Features.TodoItems.Commands.SetTodoItemAsDone;

public class SetTodoItemAsDoneCommandValidator : AbstractValidator<SetTodoItemAsDoneCommand>
{
    public SetTodoItemAsDoneCommandValidator()
    {
        RuleFor(v => v.Id).NotNull();
        RuleFor(v => v.Done).NotNull();
    }
}