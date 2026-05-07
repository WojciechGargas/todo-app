namespace Todo.Application.Commands.TodoTaskCommands;

public record AddTaskRequest(
    string Name,
    string Description);
