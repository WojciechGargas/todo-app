using Todo.Core.Exceptions;

namespace Todo.Core.ValueObjects;

public sealed record TaskName
{
    public string Value { get; }

    public TaskName(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 32)
        {
            throw new InvalidTodoTaskNameException(value);
        }
        
        Value = value;
    }
    
    public static implicit operator TaskName(string value) => new TaskName(value);
    
    public static implicit operator string(TaskName value) => value.Value;
    
    public override string ToString() => Value;
}