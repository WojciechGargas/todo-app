using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.TodoTasks.Queries.GetTasks;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Handlers;

internal sealed class GetTasksHandler(TodoDbContext dbContext) : IQueryHandler<GetTasksQuery, IReadOnlyList<TodoTaskDto>>
{
    public async Task<IReadOnlyList<TodoTaskDto>> HandleAsync(GetTasksQuery query)
    {
        var userId = new UserId(query.UserId);
        var tasks = await dbContext.TodoTasks
            .AsNoTracking()
            .Where(t => t.OwnerUserId == userId)
            .ToListAsync();


        return tasks.Select(t => t.AsDto()).ToList();
    }
}