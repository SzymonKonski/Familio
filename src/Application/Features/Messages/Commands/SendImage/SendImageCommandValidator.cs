using FluentValidation;

namespace Application.Features.Messages.Commands.SendImage;

public class SendImageCommandValidator : AbstractValidator<SendImageCommand>
{
    public SendImageCommandValidator()
    {
        RuleFor(v => v.GroupId).NotEmpty();
        RuleFor(v => v.File).NotNull();
    }
}