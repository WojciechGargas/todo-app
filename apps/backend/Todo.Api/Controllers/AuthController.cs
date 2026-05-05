using Microsoft.AspNetCore.Mvc;
using Todo.Application.Abstractions;
using Todo.Application.Commands.UserCommands;
using Todo.Application.Security;

namespace Todo.Api.Controllers;

[ApiController]
[Route("[controller]")]

public class AuthController(
    ICommandHandler<SignIn> signInHandler,
    ICommandHandler<SignUp> signUpHandler,
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
    
    [HttpPost("sign-in")]
    public async Task<ActionResult> SignIn([FromBody] SignIn command)
    {
        await signInHandler.HandleAsync(command);
        var jwt = tokenStorage.Get();
        
        return Ok(jwt);
    }
}