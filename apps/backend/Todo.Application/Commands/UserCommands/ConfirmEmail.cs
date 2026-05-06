using Todo.Application.Abstractions;

namespace Todo.Application.Commands.UserCommands;

public record ConfirmEmail(string Token) : ICommand;
