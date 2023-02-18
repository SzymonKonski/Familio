using Application.Features.Groups.Commands.AddUserToGroup;
using Application.Features.Groups.Commands.CreateGroup;
using Application.Features.Groups.Commands.RemoveUserFromGroup;
using Application.Features.Groups.Commands.SendInvitationToGroup;
using Application.Features.Groups.Commands.UpdateGroupDetail;
using Application.Features.Groups.Commands.UpdateUsernameInGroup;
using Application.Features.Groups.Queries.GetGroupDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
public class GroupsController : ApiControllerBase
{
    /// <summary>
    ///     Adds user to group by token
    /// </summary>
    /// <param name="command">AddUserToGroupCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> AddUserToGroup(AddUserToGroupCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Creates new Group
    /// </summary>
    /// <param name="command">CreateGroupCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<string>> CreateGroup(CreateGroupCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    ///     Deletes user from group
    /// </summary>
    /// <param name="command">DeleteUserFromGroupCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> DeleteUserFromGroup(DeleteUserFromGroupCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Sends email with invitation link to specified account
    /// </summary>
    /// <param name="command">SendInvitationToGroupCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> SendInvitationToGroup(SendInvitationToGroupCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Updates group information
    /// </summary>
    /// <param name="command">UpdateGroupDetailCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> Update(UpdateGroupDetailCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Updates user username in specified group
    /// </summary>
    /// <param name="command">UpdateUsernameInGroupCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> UpdateUsernameInGroup(UpdateUsernameInGroupCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Gets detailed information about group
    /// </summary>
    /// <param name="query">GetGroupDetailsQuery</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<GroupVm>> GetGroupDetails([FromQuery] GetGroupDetailsQuery query)
    {
        return await Mediator.Send(query);
    }
}