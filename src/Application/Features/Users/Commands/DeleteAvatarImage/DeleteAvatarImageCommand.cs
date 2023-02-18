using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands.DeleteAvatarImage;

[Authorize(Roles = "Parent,Child,Relative")]
public record DeleteAvatarImageCommand : IRequest;

public class DeleteAvatarImageCommandHandler : IRequestHandler<DeleteAvatarImageCommand>
{
    private readonly IAzureStorage _azureStorage;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<DomainUser> _userManager;

    public DeleteAvatarImageCommandHandler(IAzureStorage azureStorage, UserManager<DomainUser> userManager,
        ICurrentUserService currentUserService)
    {
        _azureStorage = azureStorage;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteAvatarImageCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService?.UserId == null)
            throw new NotFoundException(nameof(DomainUser), _currentUserService?.UserId);

        var user = await _userManager.Users.FirstAsync(u => u.Id == _currentUserService.UserId, cancellationToken);

        if (user.Avatar == null)
            return Unit.Value;

        await _azureStorage.DeleteAvatarImageAsync("useravatars", user.Avatar);
        user.Avatar = null;
        await _userManager.UpdateAsync(user);
        return Unit.Value;
    }
}