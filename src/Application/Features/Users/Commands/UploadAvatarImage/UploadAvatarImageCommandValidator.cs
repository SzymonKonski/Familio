using FluentValidation;

namespace Application.Features.Users.Commands.UploadAvatarImage;

public class UploadAvatarImageCommandValidator : AbstractValidator<UploadAvatarImageCommand>
{
    public UploadAvatarImageCommandValidator()
    {
        RuleFor(v => v.File).NotNull();
    }
}