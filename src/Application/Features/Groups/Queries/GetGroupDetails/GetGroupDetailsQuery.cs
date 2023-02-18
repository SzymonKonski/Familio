using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Groups.Queries.GetGroupDetails;

[Authorize(Roles = "Parent,Child,Relative")]
public record GetGroupDetailsQuery(string GroupId) : IRequest<GroupVm>;

public class GetGroupDetailsQueryHandler : IRequestHandler<GetGroupDetailsQuery, GroupVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGroupDetailsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GroupVm> Handle(GetGroupDetailsQuery request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.SingleOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
        if (group == null) throw new NotFoundException(nameof(Group), request.GroupId);

        var result = _context.Groups.Where(x => x.Id == group.Id).Include(t => t.DomainUserGroups)
            .ThenInclude(ut => ut.DomainUser).First();

        result.DomainUserGroups = result.DomainUserGroups.Where(x => x.UserRemoved == false).ToList();

        return _mapper.Map<Group, GroupVm>(result);
    }
}