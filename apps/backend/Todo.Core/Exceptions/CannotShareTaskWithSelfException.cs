namespace Todo.Core.Exceptions;

public class CannotShareTaskWithSelfException : CustomException
{
    public CannotShareTaskWithSelfException() : base("You cannot share a task with yourself.")
    {
    }
}