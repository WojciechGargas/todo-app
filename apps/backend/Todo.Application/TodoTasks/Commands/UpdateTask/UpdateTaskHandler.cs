using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.DomainServices;
using Todo.Core.Exceptions;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.UpdateTask;

public class UpdateTaskHandler(
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    ITaskService taskService) 
    : ICommandHandler<UpdateTaskCommand>
{
    public async Task HandleAsync(UpdateTaskCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);
        
        var task = await taskRepository.GetTaskByIdAsync(command.Id) ??
                           throw new TaskNotFoundException(command.Id);

        await taskService.UpdateTaskAsync(
            user,
            task,
            command.Name,
            command.Description,
            command.IsCompleted);
    }
}
