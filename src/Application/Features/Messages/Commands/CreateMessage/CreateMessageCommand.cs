using System.Text.RegularExpressions;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Hubs;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Group = Domain.Entities.Group;

namespace Application.Features.Messages.Commands.CreateMessage;

[Authorize(Roles = "Parent,Child,Relative")]
public record CreateMessageCommand : IRequest<int>
{
    public string GroupId { get; set; } = default!;

    public string Content { get; set; } = default!;
}

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly UserManager<DomainUser> _userManager;

    public CreateMessageCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        UserManager<DomainUser> userManager, INotificationService notificationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<int> Handle(CreateMessageCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == _currentUserService.UserId, cancellationToken);
        if (user == null) throw new NotFoundException(nameof(DomainUser), _currentUserService.UserId);

        var group = _context.Groups.FirstOrDefault(r => r.Id == command.GroupId);
        if (group == null) throw new NotFoundException(nameof(Group), command.GroupId);

        var domainGroup =
            await _context.DomainUserGroups.FirstOrDefaultAsync(x => x.DomainUserId == user.Id && x.GroupId == group.Id,
                cancellationToken);
        if (domainGroup == null) throw new NotFoundException(nameof(DomainUserGroup));

        var msg = new Message
        {
            Content = Regex.Replace(command.Content, @"<.*?>", string.Empty),
            ImageContent = null,
            CreatedByUser = domainGroup,
            UserId = domainGroup.DomainUserId,
            GroupId = domainGroup.GroupId,
            Timestamp = DateTime.UtcNow
        };

        _context.Messages.Add(msg);
        await _context.SaveChangesAsync(cancellationToken);

        // Broadcast the message
        var createdMessage = new MessageViewModel
        {
            Id = msg.Id,
            Content = msg.Content,
            ImageContent = null,
            UserId = domainGroup.DomainUserId,
            Timestamp = msg.Timestamp,
            GroupId = domainGroup.GroupId,
            UserAvatar = user.Avatar,
            UserName = domainGroup.UserName
        };

        await _notificationService.NewMessage(group.Id, createdMessage);

        return createdMessage.Id;
    }
}