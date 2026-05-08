using Todo.Application.Abstractions;
using Todo.Application.Email;
using Todo.Application.Exceptions;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;
using EmailAddress = Todo.Core.ValueObjects.Email;

namespace Todo.Application.Users.Commands.ConfirmEmail;

internal sealed class ConfirmEmailHandler(
    IUserRepository userRepository,
    IEmailConfirmationService emailConfirmationService) : ICommandHandler<ConfirmEmailCommand>
{
    public async Task HandleAsync(ConfirmEmailCommand command)
    {
        var payload = emailConfirmationService.ReadToken(command.Token);

        var user = await userRepository.GetUserByIdAsync(payload.UserId) ??
                   throw new UserNotFoundException(payload.UserId);

        if (payload.Purpose == EmailConfirmationPurpose.Registration)
        {
            if (user.Email != payload.Email)
            {
                throw new InvalidEmailConfirmationTokenException();
            }

            user.MarkEmailAsConfirmed();
            return;
        }

        var newEmail = new EmailAddress(payload.Email);
        var existingUser = await userRepository.GetUserByEmailAsync(newEmail);

        if (existingUser is not null && existingUser.UserId != user.UserId)
        {
            throw new EmailAlreadyInUseException(newEmail);
        }

        user.ChangeEmail(newEmail);
        user.MarkEmailAsConfirmed();
    }
}
