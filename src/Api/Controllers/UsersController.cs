using Application.Features.Users.Commands.DeleteAvatarImage;
using Application.Features.Users.Commands.UpdateUserProfile;
using Application.Features.Users.Commands.UploadAvatarImage;
using Application.Features.Users.Queries.GetUserDetails;
using Application.Features.Users.Queries.GetUserGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
public class UsersController : ApiControllerBase
{
    /// <summary>
    ///     Uploads image avatar for specified user
    /// </summary>
    /// <param name="command">UploadAvatarImageCommand</param>
    /// <returns></returns>
    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<ActionResult> UploadAvatarImage([FromForm] UploadAvatarImageCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Delete avatar image for specified user
    /// </summary>
    /// <param name="command">DeleteAvatarImageCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> DeleteAvatarImage(DeleteAvatarImageCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Updates user firstname and surname
    /// </summary>
    /// <param name="command">UpdateUserProfileCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> UpdateUserProfile(UpdateUserProfileCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    ///     Gets user information details
    /// </summary>
    /// <param name="query">GetUserDetailsQuery</param>
    /// <returns>User details information</returns>
    [HttpGet]
    public async Task<ActionResult<UserDetailsDto>> GetUserDetails([FromQuery] GetUserDetailsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    ///     Gets the list of groups to which the user belongs
    /// </summary>
    /// <param name="query">GetUserGroupsQuery</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<GroupDto>>> GetUserGroups([FromQuery] GetUserGroupsQuery query)
    {
        return await Mediator.Send(query);
    }
}