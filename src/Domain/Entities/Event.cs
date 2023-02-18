namespace Domain.Entities;

public class Event
{
    public int Id { get; set; }

    public string Description { get; set; }

    public DateTime EventStart { get; set; }

    public DateTime EventEnd { get; set; }

    public DomainUserGroup CreatedByUser { get; set; }

    public string GroupId { get; set; }

    public string UserId { get; set; }
}