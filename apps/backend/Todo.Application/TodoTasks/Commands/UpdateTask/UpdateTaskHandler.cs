using Todo.Application.Abstractions;
using Todo.Application.Exceptions;
using Todo.Core.Exceptions;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Application.TodoTasks.Commands.UpdateTask;

public class UpdateTaskHandler(ITaskRepository taskRepository) : ICommandHandler<UpdateTaskCommand>
{
    public async Task HandleAsync(UpdateTaskCommand command)
    {
        var taskToUpdate = await taskRepository.GetTaskByIdAsync(command.Id) ??
                           throw new TaskNotFoundException(command.Id);

        var userId = new UserId(command.UserId);
        
        if (taskToUpdate.OwnerUserId != userId)
            throw new TaskAccessDeniedException();
        
        if(command.Name is not null)
            taskToUpdate.ChangeName(command.Name);
        
        if(command.Description is not null)
            taskToUpdate.ChangeDescription(command.Description);

        if (command.IsCompleted is true)
            taskToUpdate.MarkAsCompleted();
        else if(command.IsCompleted is false)
            taskToUpdate.MarkAsUncompleted();
    }
}
