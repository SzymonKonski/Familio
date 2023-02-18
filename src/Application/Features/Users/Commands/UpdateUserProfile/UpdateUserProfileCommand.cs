using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands.UpdateUserProfile;

[Authorize(Roles = "Parent,Child,Relative")]
public class UpdateUserProfileCommand : IRequest
{
    public string Firstname { get; init; } = default!;

    public string Surname { get; init; } = default!;
}

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<DomainUser> _userManager;

    public UpdateUserProfileCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        UserManager<DomainUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<Unit> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
        if (user == null) throw new NotFoundException(nameof(DomainUser), _currentUserService.UserId);
        var oldName = user.DomainUsername;
        var newUserName = request.Firstname + ' ' + request.Surname;

        await _context.DatabaseFacade.ExecuteSqlRawAsync(
            "UPDATE DomainUserGroups SET UserName = {0} where UserName = {1} and DomainUserId = {2}",
            newUserName, oldName, user.Id);

        user.Firstname = request.Firstname;
        user.Surname = request.Surname;
        user.DomainUsername = newUserName;

        await _userManager.UpdateAsync(user);
        return Unit.Value;
    }
}