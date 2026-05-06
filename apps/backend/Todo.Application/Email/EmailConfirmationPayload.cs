namespace Todo.Application.Email;

public sealed record EmailConfirmationPayload(
    Guid UserId,
    string Email,
    EmailConfirmationPurpose Purpose);
