using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Schedule.Queries.GetEvents;

[Authorize(Roles = "Parent,Child,Relative")]
public record GetEventsQuery : IRequest<List<EventDto>>
{
    public string GroupId { get; set; } = default!;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}

public class
    GetEventsQueryHandler : IRequestHandler<GetEventsQuery, List<EventDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEventsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<EventDto>> Handle(GetEventsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.DateTo == default)
            request.DateTo = DateTime.MaxValue;

        var group = _context.Groups.FirstOrDefault(r => r.Id == request.GroupId);
        if (group == null) throw new NotFoundException(nameof(Group), request.GroupId);

        return await _context.Events
            .Where(x => x.GroupId == group.Id && x.EventStart > request.DateFrom && x.EventStart < request.DateTo)
            .Include(v => v.CreatedByUser)
            .OrderBy(x => x.EventStart)
            .ProjectTo<EventDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}