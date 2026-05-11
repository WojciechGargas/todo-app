using Microsoft.EntityFrameworkCore;
using Todo.Core.Entities;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Repositories;

public class TaskRepository(TodoDbContext dbContext) : ITaskRepository
{
    private readonly DbSet<TodoTask> _tasks = dbContext.TodoTasks;
    
    public Task<TodoTask?> GetTaskByIdAsync(TaskId taskId)
        => _tasks.SingleOrDefaultAsync(t => t.TaskId ==  taskId);

    public async Task AddTaskAsync(TodoTask task)
        => await _tasks.AddAsync(task);

    public Task DeleteTaskAsync(TodoTask task)
    {
        _tasks.Remove(task);
        return Task.CompletedTask;
    }
}
