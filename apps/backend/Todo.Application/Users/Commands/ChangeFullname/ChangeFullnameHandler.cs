using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;

namespace Todo.Application.Users.Commands.ChangeFullname;

public class ChangeFullnameHandler(
    IUserRepository userRepository,
    IUserService userService) : ICommandHandler<ChangeFullnameCommand>
{
    public async Task HandleAsync(ChangeFullnameCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);

        await userService.ChangeFullNameAsync(user, user, command.NewFullName);
    }
}
