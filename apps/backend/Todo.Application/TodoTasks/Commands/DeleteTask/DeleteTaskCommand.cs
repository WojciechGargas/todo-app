using Todo.Application.Abstractions;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid UserId, TaskId TaskId) : ICommand;