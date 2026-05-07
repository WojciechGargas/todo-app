using Todo.Core.ValueObjects;

namespace Todo.Application.Commands.TodoTaskCommands;

public record AddTaskRequest(
    TaskName Name,
    TaskDescription Description);