using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.Quaries;

namespace Todo.Infrastructure.DAL.Handlers;

internal sealed class GetUsersHandler(TodoDbContext dbContext) : IQueryHandler<GetUsers, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> HandleAsync(GetUsers query)
        => await dbContext.Users
            .AsNoTracking()
            .Select(u => u.AsDto())
            .ToListAsync();
}