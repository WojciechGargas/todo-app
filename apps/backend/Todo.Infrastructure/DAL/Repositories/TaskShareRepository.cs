using Microsoft.EntityFrameworkCore;
using Todo.Core.Entities;
using Todo.Core.Enums;
using Todo.Core.Repositories;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Repositories;

public class TaskShareRepository(TodoDbContext dbContext) : ITaskShareRepository
{
    private readonly DbSet<TaskShare> _taskShares = dbContext.TaskShares;

    public Task<TaskShare?> GetShareAsync(TaskId taskId, UserId userId)
        => _taskShares.SingleOrDefaultAsync(s => s.TaskId == taskId && s.UserId == userId);

    public async Task<IReadOnlyList<TaskShare>> GetSharesForTaskAsync(TaskId taskId)
        => await _taskShares.Where(s => s.TaskId == taskId).ToListAsync();

    public async Task<IReadOnlyList<TaskShare>> GetSharesForUserAsync(UserId userId)
        => await _taskShares.Where(s => s.UserId == userId).ToListAsync();

    public async Task AddShareAsync(TaskShare share)
        => await _taskShares.AddAsync(share);

    public Task DeleteShareAsync(TaskShare share)
    {
        _taskShares.Remove(share);
        return Task.CompletedTask;
    }

    public async Task UpdatePermissionAsync(TaskId taskId, UserId userId, TaskSharePermission permission)
    {
        var share = await _taskShares
            .SingleOrDefaultAsync(s => s.TaskId == taskId && s.UserId == userId) ??
                    throw new InvalidOperationException("Task share not found");
        
        share.ChangePermission(permission);
    }
}