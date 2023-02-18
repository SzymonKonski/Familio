using FluentValidation;

namespace Application.Features.Groups.Queries.GetGroupDetails;

public class GetGroupDetailsQueryValidator : AbstractValidator<GetGroupDetailsQuery>
{
    public GetGroupDetailsQueryValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
    }
}