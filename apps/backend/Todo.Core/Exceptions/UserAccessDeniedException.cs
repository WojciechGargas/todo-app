namespace Todo.Core.Exceptions;

public class UserAccessDeniedException : CustomException
{
    public UserAccessDeniedException() : base("You are not allowed to update this user.")
    {
    }
}
