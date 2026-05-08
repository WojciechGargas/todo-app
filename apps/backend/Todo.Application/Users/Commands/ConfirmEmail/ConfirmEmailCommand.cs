using Todo.Application.Abstractions;

namespace Todo.Application.Users.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string Token) : ICommand;
