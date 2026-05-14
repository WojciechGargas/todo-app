using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Application.Security;
using Todo.Core.Abstractions;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;

namespace Todo.Application.Users.Commands.SignIn;

internal sealed class SignInHandler(
    IUserRepository userRepository,
    IAuthenticator authenticator,
    IPasswordManager passwordManager,
    ITokenStorage tokenStorage,
    IClock  clock,
    IUserService  userService)
    : ICommandHandler<SignInCommand>
{
    public async Task HandleAsync(SignInCommand command)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Email) ??
                   throw new InvalidCredentialsException();

        if (!passwordManager.Validate(command.Password, user.Password))
        {
            throw new InvalidCredentialsException();
        }

        if (!user.IsEmailConfirmed)
        {
            throw new EmailNotConfirmedException(user.Email);
        }

        await userService.MarkAsLoggedInAsync(user, user, clock.CurrentTimeUtc());

        var jwt = authenticator.CreateToken(user.UserId, user.Role.ToString());

        tokenStorage.Set(jwt);
    }
}
