using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.Commands.ProfileCommands.Handlers;

public class ChangeEmailHandler(IUserRepository userRepository) : ICommandHandler<ChangeEmail>
{
    public async Task HandleAsync(ChangeEmail command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);

        if (user.Email == command.NewEmail)
            throw new EmailUnchangedException();
        
        var email = new Email(command.NewEmail);

        var isTaken = await userRepository.GetUserByEmailAsync(email);

        if (isTaken is not null && isTaken.UserId != user.UserId)
            throw new EmailAlreadyInUseException(email);
        
        user.ChangeEmail(email);
    }
}