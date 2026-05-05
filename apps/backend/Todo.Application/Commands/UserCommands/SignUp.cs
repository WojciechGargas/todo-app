using Todo.Application.Abstractions;
using Todo.Core.Enums;

namespace Todo.Application.Commands.UserCommands;

public record SignUp(
    Guid UserId,
    string Email,
    string Username,
    string Password,
    string FullName,
    UserRole? Role) : ICommand;