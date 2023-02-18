using FluentValidation;

namespace Application.Features.LocationSharing.Queries.GetUsersLocation;

public class GetUsersLocalizationQueryValidator : AbstractValidator<GetUsersLocalizationQuery>
{
    public GetUsersLocalizationQueryValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
    }
}