namespace Domain.Entities;

public class InvitationToGroup
{
    public int Id { get; set; }

    public bool Completed { get; set; }

    public string Role { get; set; }

    public string GroupId { get; set; }

    public Group Group { get; set; }

    public string DomainUserId { get; set; }

    public DomainUser DomainUser { get; set; }
}