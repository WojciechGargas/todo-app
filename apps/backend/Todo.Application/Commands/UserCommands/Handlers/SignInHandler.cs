using System.Security.Authentication;
using Todo.Application.Abstractions;
using Todo.Application.Security;
using Todo.Core.Abstractions;
using Todo.Core.Repositories;

namespace Todo.Application.Commands.UserCommands.Handlers;

internal sealed class SignInHandler(
    IUserRepository userRepository,
    IAuthenticator authenticator,
    IPasswordManager passwordManager,
    ITokenStorage tokenStorage,
    IClock clock)
    : ICommandHandler<SignIn>
{
    public async Task HandleAsync(SignIn command)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Email) ??
                   throw new InvalidCredentialException();

        if (!passwordManager.Validate(command.Password, user.Password))
        {
            throw new InvalidCredentialException();
        }
        
        var jwt = authenticator.CreateToken(user.UserId, user.Role.ToString());
        
        tokenStorage.Set(jwt);
    }
}