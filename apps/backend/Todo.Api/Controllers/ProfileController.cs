using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Auth;
using Todo.Application.Abstractions;
using Todo.Application.Users.Commands.ChangeEmail;
using Todo.Application.Users.Commands.ChangeFullname;
using Todo.Application.Users.Commands.ChangePassword;
using Todo.Application.Users.Commands.ChangeProfileVisibility;
using Todo.Application.Users.Commands.ChangeUsername;

namespace Todo.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]

public class ProfileController(
    ICommandHandler<ChangeFullnameCommand> changeFullnameHandler,
    ICommandHandler<ChangeEmailCommand> changeEmailHandler,
    ICommandHandler<ChangePasswordCommand> changePasswordHandler,
    ICommandHandler<ChangeUsernameCommand> changeUsernameHandler,
    ICommandHandler<ChangeProfileVisibilityCommand> changeProfileVisibilityHandler)
    : ControllerBase
{
    [HttpPatch("change-fullname")]
    public async Task<ActionResult> ChangeFullname(ChangeFullnameRequest request)
    {
        var userId = User.GetUserIdOrThrow();

        await changeFullnameHandler.HandleAsync(new ChangeFullnameCommand(userId, request.NewFullname));
        
        return NoContent();
    }

    [HttpPatch("change-email")]
    public async Task<ActionResult> ChangeEmail(ChangeEmailRequest request)
    {
        var userId = User.GetUserIdOrThrow();
        
        await changeEmailHandler.HandleAsync(new ChangeEmailCommand(userId, request.NewEmail));
        
        return Accepted();
    }
    
    [HttpPatch("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = User.GetUserIdOrThrow();
        
        await changePasswordHandler.HandleAsync(new ChangePasswordCommand(userId, request.OldPassword, request.NewPassword));
        
        return NoContent();
    }

    [HttpPatch("change-username")]
    public async Task<ActionResult> ChangeUsername(ChangeUsernameRequest request)
    {
        var userId = User.GetUserIdOrThrow();
        
        await changeUsernameHandler.HandleAsync(new ChangeUsernameCommand(userId, request.NewUsername));
        
        return NoContent();
    }

    [HttpPatch("change-profile-visibility")]
    public async Task<ActionResult> ChangeProfileVisibility(ChangeProfileVisibilityRequest request)
    {
        var userId = User.GetUserIdOrThrow();

        await changeProfileVisibilityHandler.HandleAsync(
            new ChangeProfileVisibilityCommand(userId, request.IsProfileVisibleForSharing));
        
        return NoContent();
    }
}
