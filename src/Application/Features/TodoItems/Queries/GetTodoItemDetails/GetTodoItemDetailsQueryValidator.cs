using FluentValidation;

namespace Application.Features.TodoItems.Queries.GetTodoItemDetails;

public class GetTodoItemDetailsQueryValidator : AbstractValidator<GetTodoItemDetailsQuery>
{
    public GetTodoItemDetailsQueryValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
        RuleFor(v => v.TodoItemId).NotNull();
    }
}