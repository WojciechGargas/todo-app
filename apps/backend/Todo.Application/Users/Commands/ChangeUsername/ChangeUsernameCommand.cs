using Todo.Application.Abstractions;

namespace Todo.Application.Users.Commands.ChangeUsername;

public record ChangeUsernameCommand(Guid UserId, string NewUsername) : ICommand;
