using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.ShareTask;

public record ShareTaskRequest(Guid TargetUserId, TaskSharePermission Permission);