using Todo.Application.Abstractions;

namespace Todo.Application.Commands.ProfileCommands;

public record ChangeEmail(Guid UserId, string NewEmail) : ICommand;
