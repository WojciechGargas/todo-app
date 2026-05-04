namespace Todo.Core.Abstractions;

public interface IClock
{
    DateTime CurrentTimeUtc();
}