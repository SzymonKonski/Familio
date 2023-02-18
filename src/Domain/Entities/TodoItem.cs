using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public PriorityLevel Priority { get; set; }

    public bool Done { get; set; }

    public string AllowedRole { get; set; }

    public string GroupId { get; set; }

    public DomainUserGroup AssignedUser { get; set; }

    public string? AssignedUserId { get; set; }

    public DomainUserGroup CreatedByUser { get; set; }

    public string UserId { get; set; }
}