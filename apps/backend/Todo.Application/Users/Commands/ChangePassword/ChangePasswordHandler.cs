using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Application.Security;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.Users.Commands.ChangePassword;

public class ChangePasswordHandler(
    IUserRepository userRepository,
    IPasswordManager passwordManager,
    IUserService userService)
    : ICommandHandler<ChangePasswordCommand>
{
    public async Task HandleAsync(ChangePasswordCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);

        if (!passwordManager.Validate(command.OldPassword, user.Password))
        {
            throw new InvalidCredentialsException();
        }

        var newPassword = new Password(command.NewPassword);
        var securedPassword = passwordManager.Secure(newPassword);
        await userService.ChangePasswordAsync(user, user, securedPassword);
    }
}
