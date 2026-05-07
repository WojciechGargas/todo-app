using Todo.Application.Abstractions;
using Todo.Core.ValueObjects;

namespace Todo.Application.Commands.TodoTaskCommands;

public record AddTask(
    TaskId Id,
    UserId UserId,
    string Name,
    string Description)
    : ICommand;
