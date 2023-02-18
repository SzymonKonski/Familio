using FluentValidation;

namespace Application.Features.TodoItems.Commands.DeleteTodoItem;

public class DeleteTodoItemCommandValidator : AbstractValidator<DeleteTodoItemCommand>
{
    public DeleteTodoItemCommandValidator()
    {
        RuleFor(v => v.Id).NotNull();
        RuleFor(v => v.GroupId).NotEmpty();
    }
}