using Todo.Application.Abstractions;

namespace Todo.Application.Commands.UserCommands;

public record SignIn(string Email, string Password) : ICommand;
