using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.LocationSharing.Queries.GetUsersLocation;

[Authorize(Roles = "Parent,Child,Relative")]
public class GetUsersLocalizationQuery : IRequest<List<UserLocalizationDto>>
{
    public string GroupId { get; init; } = default!;
}

public class GetUsersLocationQueryHandler : IRequestHandler<GetUsersLocalizationQuery, List<UserLocalizationDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetUsersLocationQueryHandler(IMapper mapper, ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<UserLocalizationDto>> Handle(GetUsersLocalizationQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService?.UserId == null)
            throw new NotFoundException(nameof(DomainUser), _currentUserService?.UserId);

        var localizations = LocalizationDictionary.Locations.Where(x => x.Key.GroupId == request.GroupId).ToList();

        await Task.CompletedTask;

        return localizations.Select(localization => new UserLocalizationDto
        {
            Latitude = localization.Value.Latitude,
            Longitude = localization.Value.Longitude,
            UserId = localization.Key.UserId,
            Error = localization.Value.Error,
            TimeStamp = localization.Value.TimeStamp
        }).ToList();
    }
}