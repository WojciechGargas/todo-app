using Todo.Application.Abstractions;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.AddTask;

public record AddTaskCommand(
    TaskId Id,
    UserId UserId,
    string Name,
    string Description)
    : ICommand;
