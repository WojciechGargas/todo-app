using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Application.Security;
using Todo.Core.Abstractions;
using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.Commands.UserCommands.Handlers;

public class SignUpHandler(
    IUserRepository userRepository,
    IPasswordManager passwordManager,
    IClock clock)
    : ICommandHandler<SignUp>
{
    public async Task HandleAsync(SignUp command)
    {
        var userId = new UserId(command.UserId);
        var email = new Email(command.Email);
        var username = new Username(command.Username);
        var password = new Password(command.Password);
        var fullname = new FullName(command.FullName);
        var role = command.Role ?? UserRole.User;

        if (await userRepository.GetUserByEmailAsync(email) is not null)
        {
            throw new EmailAlreadyInUseException(email);
        }

        if (await userRepository.GetUserByUsernameAsync(username) is not null)
        {
            throw new UsernameAlreadyInUseException(username);
        }

        var securedPassword = passwordManager.Secure(password);
        var user = new User(userId, email, username, securedPassword,
            fullname, role, clock.CurrentTimeUtc());

        
        await userRepository.AddUserAsync(user);
    }
}