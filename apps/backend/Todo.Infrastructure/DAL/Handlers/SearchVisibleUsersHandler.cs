using Microsoft.EntityFrameworkCore;
using Todo.Application.Abstractions;
using Todo.Application.DTO;
using Todo.Application.Users.Queries.SearchVisibleUsers;

namespace Todo.Infrastructure.DAL.Handlers;

public class SearchVisibleUsersHandler(TodoDbContext dbContext)
    :IQueryHandler<SearchVisibleUsersQuery, IReadOnlyList<UserShareCandidateDto>>
{
    public async Task<IReadOnlyList<UserShareCandidateDto>> HandleAsync(SearchVisibleUsersQuery query)
    {
        var users = dbContext.Users
            .AsNoTracking()
            .Where(u => u.IsProfileVisibleForSharing)
            .Where(u => u.UserId != query.RequestedUserId);

        if (!string.IsNullOrWhiteSpace(query.Query))
        {
            var q = query.Query.Trim().ToLower();

            users = users.Where(u =>
                u.Username.Value.ToLower().Contains(q));
        }

        return await users
            .OrderBy(u => u.Username.Value)
            .Select(u => new UserShareCandidateDto(
                u.UserId.Value,
                u.Username.Value,
                u.FullName.Value))
            .ToListAsync();
    }
}
