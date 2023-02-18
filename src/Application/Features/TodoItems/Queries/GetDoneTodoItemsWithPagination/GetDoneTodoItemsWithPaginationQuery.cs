using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Features.TodoItems.Queries.GetTodoItems;
using Application.Security;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TodoItems.Queries.GetDoneTodoItemsWithPagination;

[Authorize(Roles = "Parent,Child,Relative")]
public record GetDoneTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public string GroupId { get; set; } = default!;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class
    GetDoneTodoItemsWithPaginationQueryHandler : IRequestHandler<GetDoneTodoItemsWithPaginationQuery,
        PaginatedList<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDoneTodoItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemBriefDto>> Handle(GetDoneTodoItemsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var group = _context.Groups.FirstOrDefault(r => r.Id == request.GroupId);
        if (group == null) throw new NotFoundException(nameof(Group), request.GroupId);

        return await _context.TodoItems.Where(x => x.GroupId == group.Id && x.Done).Include(x => x.CreatedByUser)
            .Include(c => c.AssignedUser)
            .OrderByDescending(x => x.Created)
            .ProjectTo<TodoItemBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}