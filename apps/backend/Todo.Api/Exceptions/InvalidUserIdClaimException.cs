using Todo.Core.Exceptions;

namespace Todo.Api.Exceptions;

public class InvalidUserIdClaimException : CustomException
{
    public InvalidUserIdClaimException() : base("User ID claim is missing or invalid")
    {
    }
}