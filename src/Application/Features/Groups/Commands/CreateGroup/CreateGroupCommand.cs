using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Hubs;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Groups.Commands.CreateGroup;

[Authenticate]
public class CreateGroupCommand : IRequest<string>
{
    public string Name { get; set; } = default!;
}

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly UserManager<DomainUser> _userManager;

    public CreateGroupCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        UserManager<DomainUser> userManager, INotificationService notificationService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<string> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == _currentUserService.UserId, cancellationToken);

        if (user == null) throw new NotFoundException(nameof(DomainUser));

        // create group
        var group = new Group
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name
        };
        _context.Groups.Add(group);


        var domainUserGroup = new DomainUserGroup
        {
            DomainUser = user,
            DomainUserId = user.Id,
            Group = group,
            GroupId = group.Id,
            Role = "Parent",
            UserName = user.DomainUsername
        };
        _context.DomainUserGroups.Add(domainUserGroup);

        await _context.SaveChangesAsync(cancellationToken);
        await _notificationService.AddChatRoom(group.Id, user.Id);
        return group.Id;
    }
}