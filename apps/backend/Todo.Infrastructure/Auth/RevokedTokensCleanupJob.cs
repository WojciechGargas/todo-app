using Microsoft.EntityFrameworkCore;
using Todo.Core.Abstractions;
using Todo.Infrastructure.DAL;

namespace Todo.Infrastructure.Auth;

internal sealed class RevokedTokensCleanupJob(TodoDbContext dbContext, IClock clock)
{
    public async Task ExecuteAsync()
    {
        var now = clock.CurrentTimeUtc();

        await dbContext.RevokedTokens
            .Where(token => token.ExpiresAtUtc <= now)
            .ExecuteDeleteAsync();
    }
}