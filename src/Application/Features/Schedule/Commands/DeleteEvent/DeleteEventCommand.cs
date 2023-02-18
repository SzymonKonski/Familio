using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Schedule.Commands.DeleteEvent;

[Authorize(Roles = "Parent")]
public record DeleteEventCommand : IRequest
{
    public string GroupId { get; init; } = default!;

    public int Id { get; init; }
}

public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteEventCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await _context.Events.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (@event == null) throw new NotFoundException(nameof(Event), request.Id);
        _context.Events.Remove(@event);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}