using Todo.Core.Abstractions;

namespace Todo.Tests.Integration.Infrastructure;

internal sealed class TestClock : IClock
{
    private DateTime CurrentTime { get; set; } = new (2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    public DateTime CurrentTimeUtc() => CurrentTime;
}