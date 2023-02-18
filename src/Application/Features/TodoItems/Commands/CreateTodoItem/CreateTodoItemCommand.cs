using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TodoItems.Commands.CreateTodoItem;

[Authorize(Roles = "Parent,Child,Relative")]
public record CreateTodoItemCommand : IRequest<int>
{
    public string GroupId { get; set; } = default!;

    public string Title { get; init; } = default!;

    public string Content { get; init; } = default!;

    public PriorityLevel PriorityLevel { get; set; }

    public Role Role { get; set; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<DomainUser> _userManager;

    public CreateTodoItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        UserManager<DomainUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var group = _context.Groups.FirstOrDefault(r => r.Id == request.GroupId);
        if (group == null) throw new NotFoundException(nameof(Group), request.GroupId);

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId,
            cancellationToken);
        if (user == null) throw new NotFoundException(nameof(DomainUser), _currentUserService.UserId);

        var domainGroup =
            await _context.DomainUserGroups.FirstOrDefaultAsync(x => x.DomainUserId == user.Id && x.GroupId == group.Id,
                cancellationToken);
        if (domainGroup == null) throw new NotFoundException(nameof(DomainUserGroup));

        var entity = new TodoItem
        {
            Content = request.Content,
            GroupId = domainGroup.GroupId,
            Priority = request.PriorityLevel,
            Title = request.Title,
            Done = false,
            CreatedByUser = domainGroup,
            UserId = domainGroup.DomainUserId,
            AssignedUserId = null,
            AllowedRole = request.Role.ToString()
        };

        _context.TodoItems.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}