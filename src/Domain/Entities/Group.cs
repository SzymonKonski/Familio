using Domain.Common;

namespace Domain.Entities;

public class Group : BaseAuditableEntity
{
    public string Id { get; set; }

    public string Name { get; set; }

    public ICollection<DomainUserGroup> DomainUserGroups { get; set; }
}