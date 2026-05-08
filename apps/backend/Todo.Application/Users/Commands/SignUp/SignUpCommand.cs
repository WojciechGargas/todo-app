using Todo.Application.Abstractions;
using Todo.Core.Enums;

namespace Todo.Application.Users.Commands.SignUp;

public record SignUpCommand(
    Guid UserId,
    string Email,
    string Username,
    string Password,
    string FullName,
    UserRole? Role) : ICommand;
