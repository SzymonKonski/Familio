using FluentValidation;

namespace Application.Features.TodoItems.Commands.AssignUserToTodoItemCommand;

public class AssignUserToTodoItemCommandValidator : AbstractValidator<AssignUserToTodoItemCommand>
{
    public AssignUserToTodoItemCommandValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
        RuleFor(v => v.TodoItemId).NotNull();
    }
}