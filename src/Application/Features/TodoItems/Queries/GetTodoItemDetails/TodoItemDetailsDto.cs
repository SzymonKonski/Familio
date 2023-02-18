using Domain.Enums;

namespace Application.Features.TodoItems.Queries.GetTodoItemDetails;

public class TodoItemDetailsDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public PriorityLevel Priority { get; set; }

    public bool Done { get; set; }

    public string AllowedRole { get; set; }

    public string? AssignedUser { get; set; }

    public string? AssignedUserId { get; set; }

    public string CreatedByUser { get; set; }

    public string UserId { get; set; }
}