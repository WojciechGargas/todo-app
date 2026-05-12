using Todo.Core.Abstractions;
using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.Exceptions;
using Todo.Core.Policies;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Core.DomainServices;

public class TaskService(
    ITaskRepository taskRepository,
    ITaskDeletionPolicy deletionPolicy,
    ITaskUpdatePolicy  taskUpdatePolicy,
    ITaskShareRepository taskshareRepository,
    ITaskSharePolicy  taskSharePolicy,
    IClock clock)
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

    public async Task UpdateTaskAsync(User user, TodoTask task, string? name,
        string? description, bool? isComplete)
    {
        var share = await taskshareRepository.GetShareAsync(task.TaskId, user.UserId);
        
        if (!taskUpdatePolicy.CanUpdate(task, user , share))
            throw new TaskAccessDeniedException();
        
        if(name is not null)
            task.ChangeName(name);
        
        if(description is not null)
            task.ChangeDescription(description);

        switch (isComplete)
        {
            case true:
                task.MarkAsCompleted();
                break;
            case false:
                task.MarkAsUncompleted();
                break;
        }
    }

    public async Task ShareTaskAsync(User requestedBy, TodoTask task,
        UserId targetUserId, TaskSharePermission permission)
    {
        if (!taskSharePolicy.CanShare(task, requestedBy))
            throw new TaskAccessDeniedException();

        if (targetUserId == requestedBy.UserId)
            throw new CannotShareTaskWithSelfException();
        
        var existingShare = await taskshareRepository.GetShareAsync(task.TaskId, targetUserId);

        if (existingShare is not null)
            throw new TaskAlreadySharedException();

        var share = new TaskShare(
            task.TaskId,
            targetUserId,
            requestedBy.UserId,
            permission,
            clock.CurrentTimeUtc());
        
        await taskshareRepository.AddShareAsync(share);
    }

    public async Task UnshareTaskAsync(User requestedBy, TodoTask task, UserId targetUserId)
    {
        if (!taskSharePolicy.CanShare(task, requestedBy))
            throw new TaskAccessDeniedException();
        
        var existingShare = await taskshareRepository.GetShareAsync(task.TaskId, targetUserId);

        if (existingShare is null)
            throw new TaskShareNotFoundException();
        
        await taskshareRepository.DeleteShareAsync(existingShare);
    }
}
