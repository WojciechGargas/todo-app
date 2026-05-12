using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.UnshareTask;

public record UnshareTaskRequest(Guid TargetUserId);