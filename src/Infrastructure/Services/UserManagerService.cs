using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class UserManagerService : IUserManagerService
{
    private readonly UserManager<DomainUser> _userManager;

    public UserManagerService(UserManager<DomainUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<DomainUser> GetUserById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new NotFoundException("");
        return user;
    }
}