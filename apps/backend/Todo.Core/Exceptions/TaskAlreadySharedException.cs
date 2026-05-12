namespace Todo.Core.Exceptions;

public class TaskAlreadySharedException : CustomException
{
    public TaskAlreadySharedException() : base("This task is already shared with this user")
    {
    }
}