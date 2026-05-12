using Todo.Core.Enums;

namespace Todo.Application.TodoTasks.Commands.UpdateTaskSharePermission;

public record UpdateTaskSharePermissionRequest(Guid TargetUserId, TaskSharePermission TargetSharePermission);