using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TodoItems.Commands.AssignUserToTodoItemCommand;

[Authorize(Roles = "Parent,Child,Relative")]
public class AssignUserToTodoItemCommand : IRequest<int>
{
    public string GroupId { get; set; } = default!;

    public int TodoItemId { get; set; }

    public string? UserId { get; set; }
}

public class AssignUserToTodoItemCommandHandler : IRequestHandler<AssignUserToTodoItemCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AssignUserToTodoItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(AssignUserToTodoItemCommand request, CancellationToken cancellationToken)
    {
        var currentUserGroup =
            _context.DomainUserGroups.FirstOrDefault(r =>
                r.GroupId == request.GroupId && r.DomainUserId == _currentUserService.UserId);
        if (currentUserGroup == null) throw new NotFoundException(nameof(currentUserGroup), request.GroupId);

        var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x => x.Id == request.TodoItemId, cancellationToken);
        if (todoItem == null) throw new NotFoundException(nameof(TodoItem), request.TodoItemId);


        if (request.UserId == null)
        {
            if (currentUserGroup.Role != "Parent" && _currentUserService.UserId != todoItem.AssignedUserId)
                throw new ForbiddenAccessException("You are not allowed to assign other users to this task");
        }
        else
        {
            if (currentUserGroup.Role != "Parent" && _currentUserService.UserId != request.UserId)
                throw new ForbiddenAccessException("You are not allowed to assign other users to this task");

            var requestUserGroup =
                _context.DomainUserGroups.FirstOrDefault(r =>
                    r.GroupId == request.GroupId && r.DomainUserId == request.UserId);
            if (requestUserGroup == null) throw new NotFoundException(nameof(requestUserGroup), request.GroupId);

            if (todoItem.AllowedRole != requestUserGroup.Role)
                throw new ForbiddenAccessException($"{requestUserGroup.UserName} is not allowed to do this task");
        }

        todoItem.AssignedUserId = request.UserId;
        _context.TodoItems.Update(todoItem);
        await _context.SaveChangesAsync(cancellationToken);
        return todoItem.Id;
    }
}