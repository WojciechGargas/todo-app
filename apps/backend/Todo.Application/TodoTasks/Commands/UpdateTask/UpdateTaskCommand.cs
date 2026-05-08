using Todo.Application.Abstractions;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.UpdateTask;

public record UpdateTaskCommand(
    TaskId Id,
    Guid UserId,
    string?  Name,
    string?  Description,
    bool?  IsCompleted)
    : ICommand;