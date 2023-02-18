using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TodoItems.Commands.DeleteTodoItem;

[Authorize(Roles = "Parent,Child,Relative")]
public class DeleteTodoItemCommand : IRequest
{
    public string GroupId { get; init; } = default!;

    public int Id { get; init; }
}

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTodoItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (todoItem == null) throw new NotFoundException(nameof(TodoItem), request.Id);

        var userInGroup = await _context.DomainUserGroups.FirstOrDefaultAsync(x =>
            x.GroupId == todoItem.GroupId && x.DomainUserId == _currentUserService.UserId, cancellationToken);
        if (userInGroup == null) throw new NotFoundException(nameof(DomainUserGroup));

        if (userInGroup.Role != "Parent" && todoItem.CreatedByUser.DomainUserId != _currentUserService.UserId)
            throw new UnauthorizedAccessException("User is not allowed to delete this task");

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}