using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.Exceptions;
using Todo.Application.Quaries;
using Todo.Core.Entities;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Handlers;

internal sealed class GetTaskHandler(TodoDbContext dbContext) : IQueryHandler<GetTask, TodoTaskDto>
{
    public async Task<TodoTaskDto> HandleAsync(GetTask query)
    {
        var userId = new UserId(query.UserId);
        var task = await dbContext.Set<TodoTask>()
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.OwnerUserId == userId && a.TaskId == query.Id);

        if (task is null)
        {
            var userExists = await dbContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.UserId == userId);
            if(!userExists)
                throw new UserNotFoundException(userId);
            
            throw new TaskNotFoundException(query.Id);
        }

        return task.AsDto();
    }
}