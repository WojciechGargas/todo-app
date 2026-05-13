namespace Todo.Infrastructure.Auth;

public sealed class RevokedToken
{
    public Guid Id { get; private set; }
    public string TokenId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime RevokedAtUtc { get; private set; }

    private RevokedToken()
    {
    }
    
    public RevokedToken(string tokenId, Guid userId, DateTime expiresAtUtc)
    {
        Id = Guid.NewGuid();
        TokenId = tokenId;
        UserId = userId;
        ExpiresAtUtc = expiresAtUtc;
        RevokedAtUtc = DateTime.UtcNow;
    }
}