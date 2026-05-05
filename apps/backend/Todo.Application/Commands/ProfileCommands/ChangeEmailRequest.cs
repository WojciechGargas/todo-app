using Todo.Application.Abstractions;

namespace Todo.Application.Commands.ProfileCommands;

public record ChangeEmailRequest(string NewEmail) : ICommand;