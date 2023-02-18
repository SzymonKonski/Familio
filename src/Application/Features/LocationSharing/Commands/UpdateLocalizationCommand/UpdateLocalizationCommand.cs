using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;

namespace Application.Features.LocationSharing.Commands.UpdateLocalizationCommand;

[Authorize(Roles = "Parent,Child,Relative")]
public record UpdateLocalizationCommand : IRequest
{
    public string GroupId { get; init; }

    public double Longitude { get; init; }

    public double Latitude { get; init; }

    public double Error { get; set; }
}

public class UpdateLocalizationCommandHandler : IRequestHandler<UpdateLocalizationCommand>
{
    private readonly ICurrentUserService _currentUserService;

    public UpdateLocalizationCommandHandler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateLocalizationCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService?.UserId == null)
            throw new NotFoundException(nameof(DomainUser), _currentUserService?.UserId);

        var userId = _currentUserService.UserId;
        var location = new Localization
        {
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Error = request.Error,
            TimeStamp = DateTime.UtcNow
        };
        if (!LocalizationDictionary.Locations.TryAdd((request.GroupId, userId), location))
            LocalizationDictionary.Locations[(request.GroupId, userId)] = location;

        await Task.CompletedTask;

        return Unit.Value;
    }
}