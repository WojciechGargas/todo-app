using Todo.Core.Exceptions;

namespace Todo.Application.Exceptions;

public class UsernameAlreadyInUseException : CustomException
{
    public string Username { get; set; }
    
    public UsernameAlreadyInUseException(string username) : base($"Username '{username}' is already in use")
    {
        Username = username;
    }
}