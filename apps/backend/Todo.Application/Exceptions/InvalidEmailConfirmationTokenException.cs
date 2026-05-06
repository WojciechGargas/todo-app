using Todo.Core.Exceptions;

namespace Todo.Application.Exceptions;

public class InvalidEmailConfirmationTokenException : CustomException
{
    public InvalidEmailConfirmationTokenException() : base("Email confirmation token is invalid or expired.")
    {
    }
}
