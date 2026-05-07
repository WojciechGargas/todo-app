using Todo.Application.Abstractions;
using Todo.Core.ValueObjects;

namespace Todo.Application.Commands.TodoTaskCommands;

public record AddTask(
    TaskId Id,
    UserId UserId,
    TaskName Name,
    TaskDescription Description)
    : ICommand;