using Todo.Application.DTO;
using Todo.Core.Entities;

namespace Todo.Infrastructure.DAL.Handlers;

internal static class Extensions
{
    public static UserDto AsDto(this User entity)
    {
        return new UserDto(
            entity.UserId,
            entity.Email,
            entity.Username,
            entity.Role);
    }
    
    public static UserWithTasksDto AsWithTasksDto(this User user, IReadOnlyList<TodoTask> tasks)
    {
        var taskDtos = tasks
            .Select(t => new TodoTaskDto(
                t.TaskId,
                t.TaskName,
                t.TaskDescription,
                t.IsCompleted))
            .ToList();

        return new UserWithTasksDto(
            user.UserId,
            user.Email,
            user.Username,
            user.Role,
            taskDtos);
    }
}