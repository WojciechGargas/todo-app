using Todo.Application.Abstractions;

namespace Todo.Application.Users.Commands.ChangeFullname;

public record ChangeFullnameCommand(Guid UserId, string NewFullName) : ICommand;
