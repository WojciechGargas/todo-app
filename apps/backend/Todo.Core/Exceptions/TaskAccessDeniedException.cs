namespace Todo.Core.Exceptions;

public class TaskAccessDeniedException : CustomException
{
    public TaskAccessDeniedException() : base("You are not allowed to access this task.")
    {
    }
}