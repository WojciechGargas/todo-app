namespace Todo.Application.TodoTasks.Commands.AddTask;

public record AddTaskRequest(
    string Name,
    string Description);
