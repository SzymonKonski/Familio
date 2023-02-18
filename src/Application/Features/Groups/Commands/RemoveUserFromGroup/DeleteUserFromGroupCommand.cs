using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.LocationSharing;
using Application.Hubs;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Groups.Commands.RemoveUserFromGroup;

[Authorize(Roles = "Parent")]
public record DeleteUserFromGroupCommand : IRequest
{
    public string GroupId { get; set; } = default!;

    public string UserId { get; set; } = default!;
}

public class DeleteUserFromGroupCommandHandler : IRequestHandler<DeleteUserFromGroupCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly UserManager<DomainUser> _userManager;

    public DeleteUserFromGroupCommandHandler(IApplicationDbContext context, IIdentityService identityService,
        UserManager<DomainUser> userManager, INotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(DeleteUserFromGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FindAsync(new object[] {request.GroupId}, cancellationToken);
        if (group == null) throw new NotFoundException(nameof(Group), request.GroupId);

        var user = await _userManager.Users.FirstAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null) throw new NotFoundException(nameof(DomainUser), request.UserId);

        var userGroup = await _context.DomainUserGroups.FirstOrDefaultAsync(
            x => x.DomainUserId == request.UserId && x.GroupId == request.GroupId, cancellationToken);
        if (userGroup == null) throw new NotFoundException(nameof(DomainUserGroup), request.UserId);

        LocalizationDictionary.Locations.Remove((group.Id, user.Id));

        userGroup.UserRemoved = true;
        _context.DomainUserGroups.Update(userGroup);
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.RemoveUser(userGroup.GroupId, userGroup.DomainUserId);
        return Unit.Value;
    }
}