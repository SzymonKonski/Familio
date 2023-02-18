namespace Application.Hubs;

public class MessageViewModel
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public string? ImageContent { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
    public string GroupId { get; set; }
    public string? UserAvatar { get; set; }
    public string UserName { get; set; }
}