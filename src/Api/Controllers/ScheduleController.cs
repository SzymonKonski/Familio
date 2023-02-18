using Application.Features.Schedule.Commands.CreateEvent;
using Application.Features.Schedule.Commands.DeleteEvent;
using Application.Features.Schedule.Queries.GetEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
public class ScheduleController : ApiControllerBase
{
    /// <summary>
    ///     Creates new event in specified Group
    /// </summary>
    /// <param name="command">CreateEventCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<int>> CreateEvent(CreateEventCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    ///     Removes event from specified Group
    /// </summary>
    /// <param name="command">DeleteEventCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> DeleteEvent(DeleteEventCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Gets list of events in group
    /// </summary>
    /// <param name="query">GetEventsQuery</param>
    /// <returns>List of events</returns>
    [HttpGet]
    public async Task<ActionResult<List<EventDto>>> GetEvents([FromQuery] GetEventsQuery query)
    {
        return await Mediator.Send(query);
    }
}