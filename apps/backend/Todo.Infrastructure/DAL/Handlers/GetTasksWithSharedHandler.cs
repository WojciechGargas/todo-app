using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.TodoTasks.Queries.GetTasksWithShared;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Handlers;

internal sealed class GetTasksWithSharedHandler(TodoDbContext dbContext)
    : IQueryHandler<GetTasksWithSharedQuery, IReadOnlyList<TodoTaskDto>>
{
    public async Task<IReadOnlyList<TodoTaskDto>> HandleAsync(GetTasksWithSharedQuery query)
    {
        var userId = new UserId(query.UserId);
        var owned = dbContext.TodoTasks
            .AsNoTracking()
            .Where(t => t.OwnerUserId == userId);

        var shared = dbContext.TaskShares
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Join(
                dbContext.TodoTasks
                    .AsNoTracking(),
                s => s.TaskId,
                t => t.TaskId,
                (_, t) => t);

        var tasks = await owned
            .Union(shared)
            .Distinct()
            .ToListAsync();
        
        return tasks.Select(t => t.AsDto()).ToList();
    }
}