using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Security;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries.GetUserGroups;

[Authorize(Roles = "Parent,Child,Relative")]
public record GetUserGroupsQuery : IRequest<List<GroupDto>>;

public class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, List<GroupDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetUserGroupsQueryHandler(IApplicationDbContext context, IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<GroupDto>> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        return await _context.DomainUserGroups
            .Where(x => x.DomainUserId == _currentUserService.UserId && x.UserRemoved == false)
            .Include(ut => ut.Group)
            .ProjectToListAsync<
                GroupDto>(_mapper
                .ConfigurationProvider);
    }
}