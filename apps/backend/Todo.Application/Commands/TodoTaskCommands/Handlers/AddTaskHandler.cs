using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.Entities;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.Commands.TodoTaskCommands.Handlers;

public class AddTaskHandler(
    IUserRepository userRepository,
    ITaskRepository taskRepository) 
    : ICommandHandler<AddTask>
{
    public async Task HandleAsync(AddTask command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);

        var taskToAdd = new TodoTask(
            command.Id,
            command.UserId,
            command.Name,
            command.Description
        );
        
        await taskRepository.AddTaskAsync(taskToAdd);
        user.AddTask(taskToAdd.TaskId);
    }
}