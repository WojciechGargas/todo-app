using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.Users.Queries.GetUsers;

namespace Todo.Infrastructure.DAL.Handlers;

internal sealed class GetUsersHandler(TodoDbContext dbContext) : IQueryHandler<GetUsersQuery, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> HandleAsync(GetUsersQuery query)
        => await dbContext.Users
            .AsNoTracking()
            .Select(u => u.AsDto())
            .ToListAsync();
}
