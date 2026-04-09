namespace Todo.Core.Exceptions;

public class InvalidEmailException : CustomException
{
    public string Email { get; }
    
    public InvalidEmailException(string email) : base($"Email: {email} is not a valid email address.")
        => Email = email;
}