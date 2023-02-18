using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TodoItems.Commands.UpdateTodoItemDetail;

[Authorize(Roles = "Parent,Child,Relative")]
public record UpdateTodoItemDetailCommand : IRequest
{
    public int Id { get; init; }

    public PriorityLevel Priority { get; init; }

    public string Note { get; init; } = default!;

    public string Title { get; init; } = default!;
}

public class UpdateTodoItemDetailCommandHandler : IRequestHandler<UpdateTodoItemDetailCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTodoItemDetailCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateTodoItemDetailCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (todoItem == null) throw new NotFoundException(nameof(todoItem), request.Id);

        var userInGroup = await _context.DomainUserGroups.FirstOrDefaultAsync(x =>
            x.GroupId == todoItem.GroupId && x.DomainUserId == _currentUserService.UserId, cancellationToken);
        if (userInGroup == null) throw new NotFoundException(nameof(DomainUserGroup));

        if (userInGroup.Role != "Parent" && todoItem.AssignedUserId != _currentUserService.UserId)
            throw new UnauthorizedAccessException("User is not allowed to modify this task");

        todoItem.Priority = request.Priority;
        todoItem.Content = request.Note;
        todoItem.Title = request.Title;
        _context.TodoItems.Update(todoItem);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}