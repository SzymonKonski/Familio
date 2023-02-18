using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IUserManagerService
{
    Task<DomainUser> GetUserById(string userId);
}