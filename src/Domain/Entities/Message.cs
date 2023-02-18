namespace Domain.Entities;

public class Message
{
    public int Id { get; set; }

    public string? Content { get; set; }

    public string? ImageContent { get; set; }

    public DateTime Timestamp { get; set; }

    public DomainUserGroup CreatedByUser { get; set; }

    public string UserId { get; set; }

    public string GroupId { get; set; }
}