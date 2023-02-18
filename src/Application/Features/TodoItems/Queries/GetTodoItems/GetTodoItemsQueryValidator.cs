using FluentValidation;

namespace Application.Features.TodoItems.Queries.GetTodoItems;

public class GetTodoItemsQueryValidator : AbstractValidator<GetTodoItemsQuery>
{
    public GetTodoItemsQueryValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
    }
}