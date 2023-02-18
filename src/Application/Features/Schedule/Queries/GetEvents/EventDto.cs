namespace Application.Features.Schedule.Queries.GetEvents;

public class EventDto
{
    public int Id { get; set; }

    public DateTime EventStart { get; set; }

    public DateTime EventEnd { get; set; }

    public string Description { get; set; }

    public string CreatedByUser { get; set; }
}