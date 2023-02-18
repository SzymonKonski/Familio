using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class DomainUser : IdentityUser
{
    public ICollection<DomainUserGroup> DomainUserGroups { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public string Firstname { get; set; }
    public string Surname { get; set; }
    public string DomainUsername { get; set; }
    public string? Avatar { get; set; }
}