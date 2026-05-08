using Todo.Application.Abstractions;

namespace Todo.Application.Users.Commands.ChangePassword;

public record ChangePasswordCommand(Guid UserId, string OldPassword, string NewPassword) : ICommand;
