using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.Exceptions;
using Todo.Application.Quaries;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Handlers;

internal sealed class GetUserWithTasksHandler(TodoDbContext dbContext) : IQueryHandler<GetUserWithTasks, UserWithTasksDto>
{
    public async Task<UserWithTasksDto> HandleAsync(GetUserWithTasks query)
    {
        var userId = new UserId(query.UserId);
        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.UserId == userId);
        
        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }

        var usersTasks = await dbContext.TodoTasks
            .AsNoTracking()
            .Where(t => t.OwnerUserId == userId)
            .ToListAsync();
        
        return user.AsWithTasksDto(usersTasks);
    }
}