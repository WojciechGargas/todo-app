using Todo.Application.Abstractions;
using Todo.Application.Email;
using Todo.Application.Exceptions;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;
using EmailAddress = Todo.Core.ValueObjects.Email;

namespace Todo.Application.Commands.ProfileCommands.Handlers;

public class ChangeEmailHandler(
    IUserRepository userRepository,
    IEmailConfirmationService emailConfirmationService) : ICommandHandler<ChangeEmail>
{
    public async Task HandleAsync(ChangeEmail command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);

        if (user.Email == command.NewEmail)
        {
            throw new EmailUnchangedException();
        }

        var email = new EmailAddress(command.NewEmail);

        var isTaken = await userRepository.GetUserByEmailAsync(email);

        if (isTaken is not null && isTaken.UserId != user.UserId)
        {
            throw new EmailAlreadyInUseException(email);
        }

        await emailConfirmationService.SendEmailChangeConfirmationAsync(user.UserId, email);
    }
}
