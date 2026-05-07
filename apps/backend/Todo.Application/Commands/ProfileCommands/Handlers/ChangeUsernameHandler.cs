using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.Commands.ProfileCommands.Handlers;

public class ChangeUsernameHandler(IUserRepository userRepository) : ICommandHandler<ChangeUsername>
{
    public async Task HandleAsync(ChangeUsername command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);
        
        var username = new Username(command.NewUsername);
        var isTaken = await userRepository.GetUserByUsernameAsync(username);

        if (isTaken is not null && isTaken.UserId != user.UserId)
            throw new UsernameAlreadyInUseException(username);

        user.ChangeUsername(username);
    }
}