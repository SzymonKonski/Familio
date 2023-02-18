using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;

namespace Application.Features.Groups.Commands.UpdateGroupDetail;

[Authorize(Roles = "Parent")]
public record UpdateGroupDetailCommand : IRequest
{
    public string GroupId { get; init; } = default!;

    public string Name { get; init; } = default!;
}

public class UpdateGroupDetailCommandHandler : IRequestHandler<UpdateGroupDetailCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateGroupDetailCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateGroupDetailCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FindAsync(new object[] {request.GroupId}, cancellationToken);

        if (group == null) throw new NotFoundException(nameof(TodoItem), request.GroupId);

        group.Name = request.Name;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}