using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TodoItems.Queries.GetTodoItems;

[Authorize(Roles = "Parent,Child,Relative")]
public record GetTodoItemsQuery : IRequest<List<TodoItemBriefDto>>
{
    public string GroupId { get; set; } = default!;
}

public class
    GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery,
        List<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TodoItemBriefDto>> Handle(GetTodoItemsQuery request,
        CancellationToken cancellationToken)
    {
        var group = _context.Groups.FirstOrDefault(r => r.Id == request.GroupId);
        if (group == null) throw new NotFoundException(nameof(Group), request.GroupId);

        return await _context.TodoItems.Where(x => x.GroupId == group.Id && !x.Done).Include(x => x.CreatedByUser)
            .Include(c => c.AssignedUser)
            .OrderByDescending(x => x.Priority)
            .ProjectTo<TodoItemBriefDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}