using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Application.Security;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.Commands.ProfileCommands.Handlers;

public class ChangePasswordHandler(
    IUserRepository userRepository,
    IPasswordManager passwordManager)
    : ICommandHandler<ChangePassword>
{
    public async Task HandleAsync(ChangePassword command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);

        if (!passwordManager.Validate(command.OldPassword, user.Password))
        {
            throw new InvalidCredentialsException();
        }
        
        var newPassword = new Password(command.NewPassword);
        var securedPassword = passwordManager.Secure(newPassword);
        user.ChangePassword(securedPassword);
    }
}