using FluentValidation;

namespace Application.Features.Messages.Commands.CreateMessage;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(v => v.Content).NotEmpty();
        RuleFor(v => v.GroupId).NotEmpty();
    }
}