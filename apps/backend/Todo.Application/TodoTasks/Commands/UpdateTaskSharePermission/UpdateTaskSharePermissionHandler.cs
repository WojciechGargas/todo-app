using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;

namespace Todo.Application.TodoTasks.Commands.UpdateTaskSharePermission;

public class UpdateTaskSharePermissionHandler(
    IUserRepository userRepository,
    ITaskRepository taskRepository,
    ITaskService  taskService)
    : ICommandHandler<UpdateTaskSharePermissionCommand>
{
    public async Task HandleAsync(UpdateTaskSharePermissionCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);
        
        var task = await taskRepository.GetTaskByIdAsync(command.TaskId) ??
                   throw new TaskNotFoundException(command.TaskId);

        await taskService.UpdateTaskSharePermissionAsync(user, task,
            command.TargetUserId, command.TargetSharePermission);
    }
}