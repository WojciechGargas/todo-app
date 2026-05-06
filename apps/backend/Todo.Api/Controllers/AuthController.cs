using Microsoft.AspNetCore.Mvc;
using Todo.Application.Abstractions;
using Todo.Application.Commands.ProfileCommands;
using Todo.Application.Commands.UserCommands;
using Todo.Application.Security;

namespace Todo.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    ICommandHandler<SignIn> signInHandler,
    ICommandHandler<SignUp> signUpHandler,
    ICommandHandler<ConfirmEmail> confirmEmailHandler,
    ITokenStorage tokenStorage)
    : ControllerBase
{
    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUp([FromBody] SignUp command)
    {
        command = command with { UserId = Guid.NewGuid() };
        await signUpHandler.HandleAsync(command);

        return Created();
    }

    [HttpPost("confirm-email")]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        await confirmEmailHandler.HandleAsync(new ConfirmEmail(request.Token));
        return NoContent();
    }

    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmailFromLink([FromQuery] string token)
    {
        await confirmEmailHandler.HandleAsync(new ConfirmEmail(token));
        return Content("Email confirmed. You can close this page.");
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult> SignIn([FromBody] SignIn command)
    {
        await signInHandler.HandleAsync(command);
        var jwt = tokenStorage.Get();

        return Ok(jwt);
    }
}
