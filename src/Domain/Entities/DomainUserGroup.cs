namespace Domain.Entities;

public class DomainUserGroup
{
    public string DomainUserId { get; set; }

    public DomainUser DomainUser { get; set; }

    public string GroupId { get; set; }

    public Group Group { get; set; }

    public string UserName { get; set; }

    public string Role { get; set; }

    public bool UserRemoved { get; set; } = false;

    public ICollection<TodoItem> CreatedTodoItems { get; set; }

    public ICollection<TodoItem> AssignedTodoItems { get; set; }

    public ICollection<Event> Events { get; set; }

    public ICollection<Message> Messages { get; set; }
}