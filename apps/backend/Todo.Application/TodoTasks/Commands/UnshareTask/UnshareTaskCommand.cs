using Todo.Application.Abstractions;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.UnshareTask;

public record UnshareTaskCommand(Guid UserId, TaskId TaskId, Guid TargetUserId) : ICommand;