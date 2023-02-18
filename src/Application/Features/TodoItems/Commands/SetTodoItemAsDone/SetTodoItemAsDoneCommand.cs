using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TodoItems.Commands.SetTodoItemAsDone;

[Authorize(Roles = "Parent,Child,Relative")]
public record SetTodoItemAsDoneCommand : IRequest
{
    public int Id { get; init; }

    public bool Done { get; init; }
}

public class SetTodoItemAsDoneCommandHandler : IRequestHandler<SetTodoItemAsDoneCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public SetTodoItemAsDoneCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(SetTodoItemAsDoneCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (todoItem == null) throw new NotFoundException(nameof(TodoItem), request.Id);

        var userInGroup = await _context.DomainUserGroups.FirstOrDefaultAsync(x =>
            x.GroupId == todoItem.GroupId && x.DomainUserId == _currentUserService.UserId, cancellationToken);
        if (userInGroup == null) throw new NotFoundException(nameof(DomainUserGroup));

        if (userInGroup.Role != "Parent" && todoItem.AssignedUserId != _currentUserService.UserId)
            throw new UnauthorizedAccessException("User is not allowed to modify this task");

        todoItem.Done = request.Done;
        _context.TodoItems.Update(todoItem);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}