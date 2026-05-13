namespace Todo.Application.Security;

public interface ITokenRevocationService
{
    Task RevokeTokenAsync(string tokenId, Guid userId, DateTime expiresAtUtc);
    Task<bool> IsTokenRevokedAsync(string tokenId);
}