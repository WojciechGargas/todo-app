using Todo.Application.Abstractions;

namespace Todo.Application.Commands.ProfileCommands;

public record ChangePassword(Guid UserId, string OldPassword, string NewPassword) : ICommand;