using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;

namespace Application.Features.LocationSharing.Commands.DeleteLocalizationCommand;

[Authorize(Roles = "Parent,Child,Relative")]
public record DeleteLocalizationCommand : IRequest
{
    public string GroupId { get; init; }
}

public class DeleteLocalizationCommandHandler : IRequestHandler<DeleteLocalizationCommand>
{
    private readonly ICurrentUserService _currentUserService;

    public DeleteLocalizationCommandHandler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteLocalizationCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService?.UserId == null)
            throw new NotFoundException(nameof(DomainUser), _currentUserService?.UserId);

        var userId = _currentUserService.UserId;
        var result = LocalizationDictionary.Locations.Remove((request.GroupId, userId));
        await Task.CompletedTask;
        return Unit.Value;
    }
}