namespace Todo.Core.Exceptions;

public class TaskShareNotFoundException : CustomException
{
    public TaskShareNotFoundException() : base("Task share not found")
    {
    }
}