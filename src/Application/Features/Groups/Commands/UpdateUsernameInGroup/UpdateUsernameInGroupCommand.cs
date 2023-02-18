using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Groups.Commands.UpdateUsernameInGroup;

[Authorize(Roles = "Parent,Relative")]
public class UpdateUsernameInGroupCommand : IRequest
{
    public string UserId { get; init; } = default!;

    public string GroupId { get; init; } = default!;

    public string Username { get; init; } = default!;
}

public class UpdateUsernameInGroupCommandHandler : IRequestHandler<UpdateUsernameInGroupCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUsernameInGroupCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateUsernameInGroupCommand request, CancellationToken cancellationToken)
    {
        var userInGroup = await _context.DomainUserGroups.FirstOrDefaultAsync(x =>
            x.GroupId == request.GroupId && x.DomainUserId == request.UserId, cancellationToken);
        if (userInGroup == null) throw new NotFoundException(nameof(DomainUserGroup), request.GroupId);

        userInGroup.UserName = request.Username;
        _context.DomainUserGroups.Update(userInGroup);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}