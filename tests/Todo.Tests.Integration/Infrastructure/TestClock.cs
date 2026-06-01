using Todo.Core.Abstractions;

namespace Todo.Tests.Integration.Infrastructure;

internal sealed class TestClock : IClock
{
    private DateTime _currentTime = DateTime.UtcNow;

    public DateTime CurrentTimeUtc() => _currentTime;

    public void Set(DateTime utcDateTime) => _currentTime = utcDateTime;

    public void Advance(TimeSpan by) => _currentTime = _currentTime.Add(by);
}