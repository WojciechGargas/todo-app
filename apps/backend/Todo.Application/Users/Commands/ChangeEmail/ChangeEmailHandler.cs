using Todo.Application.Abstractions;
using Todo.Application.Email;
using Todo.Application.Exceptions;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;

namespace Todo.Application.Users.Commands.ChangeEmail;

public class ChangeEmailHandler(
    IUserRepository userRepository,
    IUserService userService,
    IEmailConfirmationService emailConfirmationService) : ICommandHandler<ChangeEmailCommand>
{
    public async Task HandleAsync(ChangeEmailCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);

        var email = await userService.PrepareEmailChangeAsync(user, user, command.NewEmail);
        await emailConfirmationService.SendEmailChangeConfirmationAsync(user.UserId, email);
    }
}
