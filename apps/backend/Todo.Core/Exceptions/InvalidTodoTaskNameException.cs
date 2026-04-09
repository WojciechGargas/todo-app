namespace Todo.Core.Exceptions;

public class InvalidTodoTaskNameException : CustomException
{
    public string TaskName { get; }
    
    public InvalidTodoTaskNameException(string taskName) : base($"Task name '{taskName}' is invalid.")
        =>  TaskName = taskName;
}