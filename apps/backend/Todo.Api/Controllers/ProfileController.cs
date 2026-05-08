using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Auth;
using Todo.Api.Exceptions;
using Todo.Application.Abstractions;
using Todo.Application.Users.Commands.ChangeEmail;
using Todo.Application.Users.Commands.ChangeFullname;

namespace Todo.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]

public class ProfileController(
    ICommandHandler<ChangeFullnameCommand> changeFullnameHandler,
    ICommandHandler<ChangeEmailCommand> changeEmailHandler)
    : ControllerBase
{
    [HttpPatch("profile/changeFullname")]
    public async Task<ActionResult> ChangeFullname(ChangeFullnameRequest request)
    {
        var userId = User.GetUserIdOrThrow();

        await changeFullnameHandler.HandleAsync(new ChangeFullnameCommand(userId, request.NewFullname));
        
        return NoContent();
    }

    [HttpPatch("profile/changeEmail")]
    public async Task<ActionResult> ChangeEmail(ChangeEmailRequest request)
    {
        var userId = User.GetUserIdOrThrow();
        
        await changeEmailHandler.HandleAsync(new ChangeEmailCommand(userId, request.NewEmail));
        
        return Accepted();
    }
}
