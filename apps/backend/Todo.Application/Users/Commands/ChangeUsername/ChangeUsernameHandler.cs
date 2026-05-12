using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;

namespace Todo.Application.Users.Commands.ChangeUsername;

public class ChangeUsernameHandler(
    IUserRepository userRepository,
    IUserService userService) : ICommandHandler<ChangeUsernameCommand>
{
    public async Task HandleAsync(ChangeUsernameCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);

        await userService.ChangeUsernameAsync(user, user, command.NewUsername);
    }
}
