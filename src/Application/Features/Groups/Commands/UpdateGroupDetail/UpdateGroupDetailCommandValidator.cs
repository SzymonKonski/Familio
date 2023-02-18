using FluentValidation;

namespace Application.Features.Groups.Commands.UpdateGroupDetail;

public class UpdateGroupDetailCommandValidator : AbstractValidator<UpdateGroupDetailCommand>
{
    public UpdateGroupDetailCommandValidator()
    {
        RuleFor(v => v.GroupId).Length(36).NotEmpty();
        RuleFor(v => v.Name).MaximumLength(200).NotEmpty();
    }
}