using FluentValidation;

namespace Application.Features.LocationSharing.Commands.DeleteLocalizationCommand;

public class DeleteLocalizationCommandValidator : AbstractValidator<DeleteLocalizationCommand>
{
    public DeleteLocalizationCommandValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
    }
}