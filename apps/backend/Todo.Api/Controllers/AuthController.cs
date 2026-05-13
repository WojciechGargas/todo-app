using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Auth;
using Todo.Application.Abstractions;
using Todo.Application.Security;
using Todo.Application.Users.Commands.ConfirmEmail;
using Todo.Application.Users.Commands.Logout;
using Todo.Application.Users.Commands.SignIn;
using Todo.Application.Users.Commands.SignUp;

namespace Todo.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    ICommandHandler<SignInCommand> signInHandler,
    ICommandHandler<SignUpCommand> signUpHandler,
    ICommandHandler<ConfirmEmailCommand> confirmEmailHandler,
    ICommandHandler<LogoutCommand> logoutHandler,
    ITokenStorage tokenStorage)
    : ControllerBase
{
    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUp([FromBody] SignUpRequest request)
    {
        var command = new SignUpCommand(
            Guid.NewGuid(),
            request.Email,
            request.Username,
            request.Password,
            request.FullName,
            request.Role);
        await signUpHandler.HandleAsync(command);

        return Created();
    }

    [HttpPost("confirm-email")]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        await confirmEmailHandler.HandleAsync(new ConfirmEmailCommand(request.Token));
        return NoContent();
    }

    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmailFromLink([FromQuery] string token)
    {
        await confirmEmailHandler.HandleAsync(new ConfirmEmailCommand(token));
        return Content("Email confirmed. You can close this page.");
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult> SignIn([FromBody] SignInRequest request)
    {
        var command = new SignInCommand(request.Email, request.Password);
        await signInHandler.HandleAsync(command);
        var jwt = tokenStorage.Get();

        return Ok(jwt);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var command = new LogoutCommand(
            User.GetUserIdOrThrow(),
            User.GetTokenIdOrThrow(),
            User.GetTokenExpirationUtcOrThrow());
            
        await logoutHandler.HandleAsync(command);
        
        return NoContent();
    }
}
