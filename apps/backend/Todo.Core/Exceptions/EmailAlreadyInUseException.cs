namespace Todo.Core.Exceptions;

public class EmailAlreadyInUseException : CustomException
{
    public string Email { get; set; }

    public EmailAlreadyInUseException(string email) : base($"Email '{email}' is already in use")
    {
        Email = email;
    }
}
