using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Messages.Queries.GetMessages;

[Authorize(Roles = "Parent,Child,Relative")]
public class GetMessagesQuery : IRequest<List<MessageDto>>
{
    public string GroupId { get; init; } = default!;
    public int? MessageId { get; set; }
}

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMessagesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(r => r.Id == request.GroupId, cancellationToken);
        if (group == null) throw new NotFoundException(nameof(Groups), request.GroupId);

        IQueryable<Message> query;


        if (request.MessageId == null)
        {
            query = _context.Messages.Where(x => x.GroupId == request.GroupId).Include(v => v.CreatedByUser)
                .OrderByDescending(x => x.Timestamp).Take(10);
        }
        else
        {
            var msg = await _context.Messages.FirstOrDefaultAsync(x => x.Id == request.MessageId, cancellationToken);
            if (msg == null) throw new NotFoundException(nameof(Message), request.MessageId);

            query = _context.Messages.Where(x => x.GroupId == request.GroupId && x.Timestamp < msg.Timestamp)
                .Include(v => v.CreatedByUser)
                .Include(v => v.CreatedByUser.DomainUser)
                .OrderByDescending(x => x.Timestamp).Take(10);
        }

        return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
    }
}