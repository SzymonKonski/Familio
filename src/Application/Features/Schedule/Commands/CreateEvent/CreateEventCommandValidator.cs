using FluentValidation;

namespace Application.Features.Schedule.Commands.CreateEvent;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(v => v.EventEnd).NotEmpty().NotNull();
        RuleFor(v => v.EventStart).NotEmpty().NotNull();
        RuleFor(v => v.Description).NotEmpty();
        RuleFor(v => v.GroupId).NotEmpty();
    }
}