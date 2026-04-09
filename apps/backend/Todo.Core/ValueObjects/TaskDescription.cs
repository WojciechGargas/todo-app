using Todo.Core.Exceptions;

namespace Todo.Core.ValueObjects;

public sealed record TaskDescription
{
    public string Value { get; }

    public TaskDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 64)
        {
            throw new InvalidTaskDescriptionException(value);
        }
        
        Value = value;
    }
    
    public static implicit operator TaskDescription(string value) => new TaskDescription(value);
    
    public static implicit operator string(TaskDescription value) => value.Value;
    
    public override string ToString() => Value;
}