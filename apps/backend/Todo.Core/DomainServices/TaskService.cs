using Todo.Core.Entities;
using Todo.Core.Exceptions;
using Todo.Core.Policies;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Core.DomainServices;

public class TaskService(
    ITaskRepository taskRepository,
    ITaskDeletionPolicy deletionPolicy)
    : ITaskService
{
    public async Task AddTaskAsync(User user, TaskId taskId, string name, string description)
    {
        var task = new TodoTask(
            taskId,
            user.UserId,
            new TaskName(name),
            new TaskDescription(description)
        );

        await taskRepository.AddTaskAsync(task);
        user.AddTask(task.TaskId);
    }

    public async Task DeleteTaskAsync(User user, TodoTask task)
    {
        if (!deletionPolicy.CanDelete(task, user))
            throw new TaskAccessDeniedException();
        
        user.RemoveTask(task.TaskId);
        await taskRepository.DeleteTaskAsync(task);
    }
}
