using Todo.Application.Abstractions;
using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.UpdateTaskSharePermission;

public record UpdateTaskSharePermissionCommand(
    Guid UserId,
    TaskId TaskId,
    Guid TargetUserId,
    TaskSharePermission TargetSharePermission)
    : ICommand;