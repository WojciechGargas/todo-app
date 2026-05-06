namespace Todo.Application.Email;

public interface IEmailConfirmationService
{
    Task SendRegistrationConfirmationAsync(Guid userId, string email);
    Task SendEmailChangeConfirmationAsync(Guid userId, string newEmail);
    EmailConfirmationPayload ReadToken(string token);
}
