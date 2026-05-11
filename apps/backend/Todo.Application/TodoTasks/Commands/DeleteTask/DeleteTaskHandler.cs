using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.DeleteTask;

public class DeleteTaskHandler(
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    ITaskService taskService)
    : ICommandHandler<DeleteTaskCommand>
{
    public async Task HandleAsync(DeleteTaskCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);
        
        var task = await taskRepository.GetTaskByIdAsync(command.TaskId) ??
                   throw new TaskNotFoundException(command.TaskId);

        if (task.OwnerUserId != user.UserId)
            throw new TaskAccessDeniedException();

        await taskService.DeleteTaskAsync(user, task);
    }
}
