using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IApplicationDbContext _dbContext;

    private readonly IUserClaimsPrincipalFactory<DomainUser> _userClaimsPrincipalFactory;
    private readonly UserManager<DomainUser> _userManager;

    public IdentityService(
        UserManager<DomainUser> userManager,
        IUserClaimsPrincipalFactory<DomainUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _dbContext = dbContext;
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) throw new NotFoundException(nameof(DomainUser), userId);

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, string groupId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new NotFoundException(nameof(DomainUser), userId);

        var row = await _dbContext.DomainUserGroups
            .FirstOrDefaultAsync(x => x.GroupId == groupId
                                      && x.DomainUserId == userId &&
                                      x.Role == role);

        return row is {UserRemoved: false};
    }
}