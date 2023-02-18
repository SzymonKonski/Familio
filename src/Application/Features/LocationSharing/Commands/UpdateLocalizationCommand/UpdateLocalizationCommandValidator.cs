using FluentValidation;

namespace Application.Features.LocationSharing.Commands.UpdateLocalizationCommand;

public class UpdateLocalizationCommandValidator : AbstractValidator<UpdateLocalizationCommand>
{
    public UpdateLocalizationCommandValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
    }
}