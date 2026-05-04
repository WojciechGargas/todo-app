using Todo.Core.Exceptions;

namespace Todo.Application.Exceptions;

public class InvalidCredentialsException : CustomException
{
    public InvalidCredentialsException() : base("Invalid credentials")
    {
    }
}