using Todo.Application.Abstractions;

namespace Todo.Application.Users.Commands.SignIn;

public record SignInCommand(string Email, string Password) : ICommand;
