using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;

namespace Todo.Application.Users.Commands.ChangeProfileVisibility;

public class ChangeProfileVisibilityHandler(
    IUserRepository userRepository,
    IUserService userService)
    :ICommandHandler<ChangeProfileVisibilityCommand>
{
    public async Task HandleAsync(ChangeProfileVisibilityCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId)
                   ?? throw new UserNotFoundException(command.UserId);

        await userService.ChangeProfileVisibilityAsync(user, user, command.IsProfileVisibleForSharing);
    }
}