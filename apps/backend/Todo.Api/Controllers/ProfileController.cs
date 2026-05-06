using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Exceptions;
using Todo.Application.Abstractions;
using Todo.Application.Commands.ProfileCommands;

namespace Todo.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]

public class ProfileController(
    ICommandHandler<ChangeFullname> changeFullnameHandler,
    ICommandHandler<ChangeEmail> changeEmailHandler)
    : ControllerBase
{
    [HttpPatch("profile/changeFullname")]
    public async Task<ActionResult> ChangeFullname(ChangeFullnameRequest request)
    {
        var userId = GetUserId();

        await changeFullnameHandler.HandleAsync(new ChangeFullname(userId, request.NewFullname));
        
        return NoContent();
    }

    [HttpPatch("profile/changeEmail")]
    public async Task<ActionResult> ChangeEmail(ChangeEmailRequest request)
    {
        var userId = GetUserId();
        
        await changeEmailHandler.HandleAsync(new ChangeEmail(userId, request.NewEmail));
        
        return Accepted();
    }
    
    private Guid GetUserId()
    {
        if (!Guid.TryParse(User.Identity?.Name, out var userId))
        {
            throw new InvalidUserIdClaimException();
        }
        
        return userId;
    }
}
