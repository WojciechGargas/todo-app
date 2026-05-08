using Todo.Core.Exceptions;

namespace Todo.Application.Exceptions;

public class TaskAccessDeniedException : CustomException
{
    public TaskAccessDeniedException() : base("You do not have permission to modify this task.")
    {
    }
}