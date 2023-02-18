using Application.Features.LocationSharing.Commands.DeleteLocalizationCommand;
using Application.Features.LocationSharing.Commands.UpdateLocalizationCommand;
using Application.Features.LocationSharing.Queries.GetUsersLocation;
using Application.Security;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
public class LocalizationController : ApiControllerBase
{
    /// <summary>
    ///     Update user localization for specified group
    /// </summary>
    /// <param name="command">UpdateLocalizationCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<int>> UpdateLocalizationInGroup(UpdateLocalizationCommand command)
    {
        var result = await Mediator.Send(command);

        return Ok(result);
    }

    /// <summary>
    ///     Delete user localization for specified group
    /// </summary>
    /// <param name="command">DeleteLocalizationCommand</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<int>> DeleteYourLocalizationFromGroup(DeleteLocalizationCommand command)
    {
        var result = await Mediator.Send(command);

        return Ok(result);
    }

    /// <summary>
    ///     Get localization of users in group
    /// </summary>
    /// <param name="query">GetUsersLocalizationQuery</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<UserLocalizationDto>>> GetUsersLocalization(
        [FromQuery] GetUsersLocalizationQuery query)
    {
        var result = await Mediator.Send(query);


        return Ok(result);
    }
}