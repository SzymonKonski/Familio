using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TodoItems.Queries.GetTodoItemDetails;

[Authorize(Roles = "Parent,Child,Relative")]
public record GetTodoItemDetailsQuery : IRequest<TodoItemDetailsDto>
{
    public string GroupId { get; set; } = default!;

    public int TodoItemId { get; set; }
}

public class
    GetTodoItemDetailsQueryHandler : IRequestHandler<GetTodoItemDetailsQuery, TodoItemDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemDetailsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodoItemDetailsDto> Handle(GetTodoItemDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(r => r.Id == request.GroupId, cancellationToken);
        if (group == null) throw new NotFoundException(nameof(Group), request.GroupId);

        var todoItem = await _context.TodoItems.Where(x => x.Id == request.TodoItemId).Include(v => v.CreatedByUser)
            .Include(c => c.AssignedUser)
            .ToListAsync(cancellationToken);
        if (todoItem == null) throw new NotFoundException(nameof(TodoItem), request.TodoItemId);

        return _mapper.Map<TodoItem, TodoItemDetailsDto>(todoItem.First());
    }
}