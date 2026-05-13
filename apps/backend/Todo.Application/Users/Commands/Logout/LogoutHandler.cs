using Todo.Application.Abstractions;
using Todo.Application.Security;

namespace Todo.Application.Users.Commands.Logout;

public class LogoutHandler(ITokenRevocationService tokenRevocationService)
    : ICommandHandler<LogoutCommand>
{
    public Task HandleAsync(LogoutCommand command)
        => tokenRevocationService.RevokeTokenAsync(
            command.TokenId,
            command.UserId,
            command.ExpiresAtUtc);
}