namespace Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(string userId, string role, string groupId);

    Task<bool> AuthorizeAsync(string userId, string policyName);
}