using Todo.Application.Abstractions;

namespace Todo.Application.Commands.ProfileCommands;

public record ChangeFullname(Guid UserId, string NewFullName) : ICommand;