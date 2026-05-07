using Todo.Core.Exceptions;
using Todo.Core.ValueObjects;

namespace Todo.Application.Exceptions;

public class TaskNotFoundException : CustomException
{
    public TaskId TaskId;
    
    public TaskNotFoundException(TaskId taskId) : base($"Task with ID : '{taskId}' was not found.")
    {
        TaskId = taskId;
    }
}