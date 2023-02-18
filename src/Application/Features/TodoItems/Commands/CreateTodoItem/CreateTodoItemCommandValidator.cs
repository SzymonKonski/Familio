using FluentValidation;

namespace Application.Features.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();
        RuleFor(v => v.Content).NotEmpty();
        RuleFor(v => v.GroupId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum();
        RuleFor(x => x.PriorityLevel).IsInEnum();
    }
}