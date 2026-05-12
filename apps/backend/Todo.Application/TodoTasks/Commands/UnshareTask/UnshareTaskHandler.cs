using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;

namespace Todo.Application.TodoTasks.Commands.UnshareTask;

public class UnshareTaskHandler(
    IUserRepository userRepository,
    ITaskRepository taskRepository,
    ITaskService taskService)
    : ICommandHandler<UnshareTaskCommand>
{
    public async Task HandleAsync(UnshareTaskCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);
        
        var task = await taskRepository.GetTaskByIdAsync(command.TaskId) ??
                   throw new TaskNotFoundException(command.TaskId);
        
        await taskService.UnshareTaskAsync(user, task,  command.TargetUserId);
    }
}