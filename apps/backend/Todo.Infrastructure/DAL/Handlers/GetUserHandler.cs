using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.Exceptions;
using Todo.Application.Quaries;
using Todo.Core.ValueObjects;

namespace Todo.Infrastructure.DAL.Handlers;

internal sealed class GetUserHandler(TodoDbContext dbContext) : IQueryHandler<GetUser, UserDto>
{
    public async Task<UserDto> HandleAsync(GetUser query)
    {
        var userId = new UserId(query.UserId);
        var user = await dbContext.Users
            .Include(a => a.TaskIds)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }

        return user.AsDto();
    }
}