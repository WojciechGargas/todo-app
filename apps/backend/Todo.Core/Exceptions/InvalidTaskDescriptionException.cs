namespace Todo.Core.Exceptions;

public class InvalidTaskDescriptionException : CustomException
{
    public string TaskDescription { get; }
    
    public InvalidTaskDescriptionException(string taskDescription) : base($"Task description '{taskDescription}' is invalid.")
        =>  TaskDescription = taskDescription;
}