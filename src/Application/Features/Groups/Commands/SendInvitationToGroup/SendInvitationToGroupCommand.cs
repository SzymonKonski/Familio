using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Groups.Commands.SendInvitationToGroup;

[Authorize(Roles = "Parent")]
public record SendInvitationToGroupCommand : IRequest
{
    public string Email { get; init; } = default!;
    public string GroupId { get; set; } = default!;
    public Role Role { get; set; }
}

public class SendInvitationToGroupCommandHandler : IRequestHandler<SendInvitationToGroupCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailCallbackService _emailCallbackService;
    private readonly UserManager<DomainUser> _userManager;

    public SendInvitationToGroupCommandHandler(IApplicationDbContext context,
        UserManager<DomainUser> userManager, IEmailCallbackService emailCallbackService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _userManager = userManager;
        _emailCallbackService = emailCallbackService;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(SendInvitationToGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null) throw new NotFoundException(nameof(DomainUser), request.Email);

        if (!await _userManager.IsEmailConfirmedAsync(user)) throw new Exception("User is not confirmed");

        var group = await _context.Groups.FirstAsync(x => x.Id == request.GroupId, cancellationToken);
        if (group == null) throw new NotFoundException(nameof(Group), request.GroupId);

        var currentUserGroup = await _context.DomainUserGroups.FirstOrDefaultAsync(x =>
            x.DomainUserId == _currentUserService.UserId && x.GroupId == request.GroupId, cancellationToken);
        if (currentUserGroup == null) throw new NotFoundException(nameof(DomainUserGroup));

        var userGroup = await _context.DomainUserGroups.FirstOrDefaultAsync(x =>
            x.DomainUserId == user.Id && x.GroupId == request.GroupId, cancellationToken);

        if (userGroup is {UserRemoved: false})
            throw new Exception("User is already in group");

        var invitation = new InvitationToGroup
        {
            Role = request.Role.ToString(),
            DomainUser = user,
            DomainUserId = user.Id,
            GroupId = group.Id,
            Group = group,
            Completed = false
        };

        _context.Invitations.Add(invitation);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _emailCallbackService.SendInvitationLink(user, group);

        if (!result)
            throw new EmailNotSendException("Email was not send.");

        return Unit.Value;
    }
}