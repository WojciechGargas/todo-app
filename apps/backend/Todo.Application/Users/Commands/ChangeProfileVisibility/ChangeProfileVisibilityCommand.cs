using Todo.Application.Abstractions;
using Todo.Core.ValueObjects;

namespace Todo.Application.Users.Commands.ChangeProfileVisibility;

public record ChangeProfileVisibilityCommand(UserId UserId, bool IsProfileVisibleForSharing) : ICommand;