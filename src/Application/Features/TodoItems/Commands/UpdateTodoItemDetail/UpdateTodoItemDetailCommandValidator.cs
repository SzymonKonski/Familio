using FluentValidation;

namespace Application.Features.TodoItems.Commands.UpdateTodoItemDetail;

public class UpdateTodoItemDetailCommandValidator : AbstractValidator<UpdateTodoItemDetailCommand>
{
    public UpdateTodoItemDetailCommandValidator()
    {
        RuleFor(v => v.Note).NotEmpty();
        RuleFor(v => v.Title).NotEmpty();
        RuleFor(v => v.Priority).NotNull();
        RuleFor(v => v.Id).NotNull();
    }
}