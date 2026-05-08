using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.Users.Commands.ChangeFullname;

public class ChangeFullnameHandler(IUserRepository userRepository) : ICommandHandler<ChangeFullnameCommand>
{
    public async Task HandleAsync(ChangeFullnameCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);
        
        var fullName = new FullName(command.NewFullName);
        
        user.ChangeFullName(fullName);
    }
}
