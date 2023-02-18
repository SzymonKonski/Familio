using FluentValidation;

namespace Application.Features.Schedule.Commands.DeleteEvent;

public class DeleteEventCommandValidator : AbstractValidator<DeleteEventCommand>
{
    public DeleteEventCommandValidator()
    {
        RuleFor(v => v.Id).NotNull();
        RuleFor(v => v.GroupId).NotEmpty();
    }
}