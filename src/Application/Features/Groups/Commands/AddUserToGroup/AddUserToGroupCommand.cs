using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Hubs;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Groups.Commands.AddUserToGroup;

public record AddUserToGroupCommand : IRequest
{
    public string UserId { get; set; } = default!;

    public string Code { get; set; } = default!;
}

public class AddUserToGroupCommandHandler : IRequestHandler<AddUserToGroupCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IInvitationDataProtector _dataProtector;
    private readonly IMobileCodeService _mobileCodeService;
    private readonly INotificationService _notificationService;
    private readonly UserManager<DomainUser> _userManager;

    public AddUserToGroupCommandHandler(UserManager<DomainUser> userManager, IInvitationDataProtector dataProtector,
        IApplicationDbContext context, INotificationService notificationService, IMobileCodeService mobileCodeService)
    {
        _userManager = userManager;
        _dataProtector = dataProtector;
        _context = context;
        _notificationService = notificationService;
        _mobileCodeService = mobileCodeService;
    }

    public async Task<Unit> Handle(AddUserToGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null) throw new NotFoundException(nameof(DomainUser), request.UserId);

        var token = await _mobileCodeService.VerifyCode(user.Id, AuthActionType.SendInvitation, request.Code);
        if (token == string.Empty)
            throw new Exception("Invalid token");

        var result = await _userManager.VerifyUserTokenAsync(user, "CustomTokenProvider", "Invitation", token);

        if (!result)
            throw new Exception("Could not add user to group");

        var invitation = await _dataProtector.GetDataFromToken(token);
        var groupId = invitation.GroupId;
        var role = invitation.Role;

        var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);
        if (group == null) throw new NotFoundException(nameof(Group), groupId);

        var userGroup = await _context.DomainUserGroups.FirstOrDefaultAsync(x =>
            x.DomainUserId == user.Id && x.GroupId == group.Id, cancellationToken);

        if (userGroup != null)
        {
            userGroup.UserRemoved = false;
            _context.DomainUserGroups.Update(userGroup);
        }
        else
        {
            var domainUserGroup = new DomainUserGroup
            {
                DomainUser = user,
                DomainUserId = user.Id,
                Group = group,
                GroupId = groupId,
                Role = role,
                UserName = user.DomainUsername,
                UserRemoved = false
            };

            _context.DomainUserGroups.Add(domainUserGroup);
        }

        _context.Invitations.Remove(invitation);
        await _context.SaveChangesAsync(cancellationToken);
        await _notificationService.AddUser(groupId, user.Id);
        return Unit.Value;
    }
}