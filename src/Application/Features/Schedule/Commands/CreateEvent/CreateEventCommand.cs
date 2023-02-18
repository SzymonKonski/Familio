using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Schedule.Commands.CreateEvent;

[Authorize(Roles = "Parent")]
public class CreateEventCommand : IRequest<int>
{
    public DateTime EventStart { get; set; }

    public DateTime EventEnd { get; set; }

    public string GroupId { get; set; } = default!;

    public string Description { get; set; } = default!;
}

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<DomainUser> _userManager;

    public CreateEventCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        UserManager<DomainUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<int> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(r => r.Id == request.GroupId, cancellationToken);
        if (group == null) throw new NotFoundException(nameof(Group), request.GroupId);

        var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
        if (user == null) throw new NotFoundException(nameof(DomainUser), _currentUserService.UserId);

        var domainGroup =
            await _context.DomainUserGroups.FirstOrDefaultAsync(x => x.DomainUserId == user.Id && x.GroupId == group.Id,
                cancellationToken);
        if (domainGroup == null) throw new NotFoundException(nameof(DomainUserGroup));

        var entity = new Event
        {
            GroupId = domainGroup.GroupId,
            CreatedByUser = domainGroup,
            UserId = domainGroup.DomainUserId,
            EventEnd = request.EventEnd,
            EventStart = request.EventStart,
            Description = request.Description
        };

        _context.Events.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}