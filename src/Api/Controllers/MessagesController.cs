using Application.Features.Messages.Commands.CreateMessage;
using Application.Features.Messages.Commands.SendImage;
using Application.Features.Messages.Queries.GetMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
public class MessagesController : ApiControllerBase
{
    /// <summary>
    ///     Gets the list of messages in specified group
    /// </summary>
    /// <param name="query">GetMessagesQuery</param>
    /// <returns>List of messages in group</returns>
    [HttpGet]
    public async Task<ActionResult<List<MessageDto>>> GetMessages([FromQuery] GetMessagesQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    ///     Creates new message in specified group
    /// </summary>
    /// <param name="command">CreateMessageCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateMessageCommand command)
    {
        var result = await Mediator.Send(command);

        return Ok(result);
    }

    //[HttpGet("{id}")]
    //public async Task<ActionResult<MessageViewModel>> Get(int id)
    //{
    //    return await Mediator.Send(new GetMessageQuery(id));
    //}

    /// <summary>
    ///     Creates new message which is an image
    /// </summary>
    /// <param name="command">SendImageCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<int>> SendImage([FromForm] SendImageCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}