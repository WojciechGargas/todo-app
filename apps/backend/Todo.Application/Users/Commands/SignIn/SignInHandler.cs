using System.Security.Authentication;
using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Application.Security;
using Todo.Core.Repositories;

namespace Todo.Application.Users.Commands.SignIn;

internal sealed class SignInHandler(
    IUserRepository userRepository,
    IAuthenticator authenticator,
    IPasswordManager passwordManager,
    ITokenStorage tokenStorage)
    : ICommandHandler<SignInCommand>
{
    public async Task HandleAsync(SignInCommand command)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Email) ??
                   throw new InvalidCredentialException();

        if (!passwordManager.Validate(command.Password, user.Password))
        {
            throw new InvalidCredentialException();
        }

        if (!user.IsEmailConfirmed)
        {
            throw new EmailNotConfirmedException(user.Email);
        }

        //todo: see if u want to mark as logged in

        var jwt = authenticator.CreateToken(user.UserId, user.Role.ToString());

        tokenStorage.Set(jwt);
    }
}
