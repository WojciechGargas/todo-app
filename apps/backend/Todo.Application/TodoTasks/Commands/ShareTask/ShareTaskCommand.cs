using Todo.Application.Abstractions;
using Todo.Core.Enums;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.ShareTask;

public record ShareTaskCommand(Guid UserId, TaskId TaskId, Guid TargetUserId, TaskSharePermission Permission) : ICommand;