using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.Entities;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.AddTask;

public class AddTaskHandler(
    IUserRepository userRepository,
    ITaskRepository taskRepository) 
    : ICommandHandler<AddTaskCommand>
{
    public async Task HandleAsync(AddTaskCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);

        var taskToAdd = new TodoTask(
            command.Id,
            command.UserId,
            new TaskName(command.Name),
            new TaskDescription(command.Description)
        );
        
        await taskRepository.AddTaskAsync(taskToAdd);
        user.AddTask(taskToAdd.TaskId);
    }
}
