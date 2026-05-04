using Todo.Core.Abstractions;

namespace Todo.Infrastructure.Time;

public class Clock : IClock
{
    public DateTime CurrentTimeUtc()
        => DateTime.UtcNow;
}