using Todo.Core.Exceptions;

namespace Todo.Application.Exceptions;

public class EmailNotConfirmedException : CustomException
{
    public string Email { get; }

    public EmailNotConfirmedException(string email) : base($"Email '{email}' is not confirmed.")
    {
        Email = email;
    }
}
