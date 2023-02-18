using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ValidationException = FluentValidation.ValidationException;

namespace Application.Features.Users.Commands.UploadAvatarImage;

[Authorize(Roles = "Parent,Child,Relative")]
public record UploadAvatarImageCommand : IRequest
{
    public IFormFile File { get; set; } = default!;
}

public class UploadAvatarImageCommandHandler : IRequestHandler<UploadAvatarImageCommand>
{
    private readonly IAzureStorage _azureStorage;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileValidator _fileValidator;
    private readonly UserManager<DomainUser> _userManager;

    public UploadAvatarImageCommandHandler(IAzureStorage azureStorage, UserManager<DomainUser> userManager,
        ICurrentUserService currentUserService, IFileValidator fileValidator)
    {
        _azureStorage = azureStorage;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _fileValidator = fileValidator;
    }

    public async Task<Unit> Handle(UploadAvatarImageCommand request, CancellationToken cancellationToken)
    {
        var file = request.File;

        if (!_fileValidator.IsValid(file))
            throw new ValidationException("File is not valid");

        if (_currentUserService?.UserId == null)
            throw new NotFoundException(nameof(DomainUser), _currentUserService?.UserId);

        var user = await _userManager.Users.FirstAsync(u => u.Id == _currentUserService.UserId, cancellationToken);
        var fileUrl = await _azureStorage.UploadUserAvatarAsync(file, "useravatars", user.Avatar);

        user.Avatar = fileUrl;
        await _userManager.UpdateAsync(user);

        return Unit.Value;
    }
}