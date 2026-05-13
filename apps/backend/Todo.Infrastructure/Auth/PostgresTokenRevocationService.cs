using Microsoft.EntityFrameworkCore;
using Todo.Application.Security;
using Todo.Infrastructure.DAL;

namespace Todo.Infrastructure.Auth;

internal sealed class PostgresTokenRevocationService(TodoDbContext dbContext) : ITokenRevocationService
{
    public async Task RevokeTokenAsync(string tokenId, Guid userId, DateTime expiresAtUtc)
    {
        if (await dbContext.RevokedTokens.AnyAsync(x => x.TokenId == tokenId))
            return;
        
        await dbContext.RevokedTokens.AddAsync(new RevokedToken(tokenId, userId, expiresAtUtc));
    }

    public Task<bool> IsTokenRevokedAsync(string tokenId)
        => dbContext.RevokedTokens.AnyAsync(x => x.TokenId == tokenId);
}