using Todo.Application.Abstractions;

namespace Todo.Application.Users.Commands.Logout;

public record LogoutCommand(Guid UserId, string TokenId, DateTime ExpiresAtUtc) : ICommand
{
}