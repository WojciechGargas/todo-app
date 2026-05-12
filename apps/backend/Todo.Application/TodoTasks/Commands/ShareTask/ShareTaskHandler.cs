using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;

namespace Todo.Application.TodoTasks.Commands.ShareTask;

public class ShareTaskHandler(
    IUserRepository userRepository,
    ITaskRepository  taskRepository,
    ITaskService taskService) 
    : ICommandHandler<ShareTaskCommand>
{
    public async Task HandleAsync(ShareTaskCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);
        
        var task = await taskRepository.GetTaskByIdAsync(command.TaskId) ??
                   throw new TaskNotFoundException(command.TaskId);
        
        await taskService.ShareTaskAsync(user, task, command.TargetUserId, command.Permission);
    }
}