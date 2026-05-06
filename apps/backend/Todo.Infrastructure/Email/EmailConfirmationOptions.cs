namespace Todo.Infrastructure.Email;

public class EmailConfirmationOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public string FrontendConfirmUrl { get; set; } = string.Empty;
    public TimeSpan? Expiry { get; set; }
}
