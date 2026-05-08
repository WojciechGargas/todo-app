using Todo.Application.Abstractions;

namespace Todo.Application.Users.Commands.ChangeEmail;

public record ChangeEmailCommand(Guid UserId, string NewEmail) : ICommand;
