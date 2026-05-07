using Todo.Application.Abstractions;

namespace Todo.Application.Commands.ProfileCommands;

public record ChangeUsername(Guid UserId, string NewUsername) : ICommand;