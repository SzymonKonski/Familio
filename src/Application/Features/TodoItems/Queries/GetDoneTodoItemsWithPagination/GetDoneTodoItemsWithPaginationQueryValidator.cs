using FluentValidation;

namespace Application.Features.TodoItems.Queries.GetDoneTodoItemsWithPagination;

public class GetDoneTodoItemsWithPaginationQueryValidator : AbstractValidator<GetDoneTodoItemsWithPaginationQuery>
{
    public GetDoneTodoItemsWithPaginationQueryValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
    }
}