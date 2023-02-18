using Application.Common.Models;
using Application.Features.Auth.Commands.ConfirmEmail;
using Application.Features.Auth.Commands.CreateToken;
using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.ResendVerificationEmail;
using Application.Features.Auth.Commands.ResetPassword;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ApiControllerBase
{
    /// <summary>
    ///     Register an account
    /// </summary>
    /// <param name="command">RegisterCommand</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IEnumerable<string>), 400)]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Succeeded)
            return Ok(result.SuccessParam);

        return BadRequest(result.Errors);
    }

    /// <summary>
    ///     Confirms an user email address
    /// </summary>
    /// <param name="command">ConfirmEmailCommand</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IEnumerable<string>), 400)]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    /// <summary>
    ///     Resend verification email with uri that contains token
    /// </summary>
    /// <param name="command">ResendVerificationEmailCommand</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(IEnumerable<string>), 400)]
    public async Task<IActionResult> ResendVerificationEmail(ResendVerificationEmailCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    /// <summary>
    ///     Get access_token and refresh_token
    /// </summary>
    /// <param name="command">CreateTokenCommand</param>
    /// <returns>access_token and refresh_token</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TokenModel), 200)]
    [ProducesResponseType(typeof(IEnumerable<string>), 400)]
    public async Task<ActionResult> CreateToken(CreateTokenCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Success)
            return Ok(new TokenModel {RefreshToken = result.RefreshToken, AccessToken = result.AccessToken});

        return BadRequest(result.Errors);
    }


    /// <summary>
    ///     Refresh access_token using refresh_token
    /// </summary>
    /// <param name="command">RefreshTokenCommand</param>
    /// <returns>new access_token and new refresh_token</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TokenModel), 200)]
    [ProducesResponseType(typeof(IEnumerable<string>), 400)]
    public async Task<ActionResult> RefreshToken(RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Success)
            return Ok(new TokenModel {RefreshToken = result.RefreshToken, AccessToken = result.AccessToken});

        return BadRequest(result.Errors);
    }

    /// <summary>
    ///     Reset account password with reset token
    /// </summary>
    /// <param name="command">ResetPasswordCommand</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(IdentityResult), 200)]
    [ProducesResponseType(typeof(IEnumerable<string>), 400)]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    /// <summary>
    ///     Sends email with an uri that contains reset token
    /// </summary>
    /// <param name="command">ForgotPasswordCommand</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(IEnumerable<string>), 400)]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Succeeded)
            return Ok(result.SuccessParam);

        return BadRequest(result.Errors);
    }
}