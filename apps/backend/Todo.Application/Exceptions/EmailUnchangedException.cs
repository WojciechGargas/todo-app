using Todo.Core.Exceptions;

namespace Todo.Application.Exceptions;

public class EmailUnchangedException : CustomException
{
    public EmailUnchangedException() : base("New email must be different from the current email")
    {
    }
}