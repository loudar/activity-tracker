namespace LifeMonitor;

public record TrackedApplication
{
    public string? ProcessName { get; set; }
    public string? MachineName { get; set; }
    public string? WindowTitle { get; set; }
    public DateTime Timestamp { get; set; }
}