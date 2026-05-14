namespace Todo.Infrastructure.Hangfire;

internal sealed class HangfireOptions
{
    public const string SectionName = "Hangfire";
    public string schema { get; init; } = "hangfire";
    public string CleanupCron { get; init; } = "0 12 * * *"; // runs every day at 12:00
    public bool DashboardEnabled { get; init; } = true;
}