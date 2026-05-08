using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.DomainServices;
using Todo.Core.Repositories;

namespace Todo.Application.TodoTasks.Commands.AddTask;

public class AddTaskHandler(
    IUserRepository userRepository,
    ITaskService taskService)
    : ICommandHandler<AddTaskCommand>
{
    public async Task HandleAsync(AddTaskCommand command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId) ??
                   throw new UserNotFoundException(command.UserId);

        await taskService.AddTaskAsync(user, command.Id, command.Name, command.Description);
    }
}
