using FluentValidation;

namespace Application.Features.Schedule.Queries.GetEvents;

public class GetEventsQueryValidator : AbstractValidator<GetEventsQuery>
{
    public GetEventsQueryValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
    }
}