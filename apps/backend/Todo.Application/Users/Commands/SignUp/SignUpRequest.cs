using Todo.Core.Enums;

namespace Todo.Application.Users.Commands.SignUp;

public record SignUpRequest(
    string Email,
    string Username,
    string Password,
    string FullName,
    UserRole? Role);
