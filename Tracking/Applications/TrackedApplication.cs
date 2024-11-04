using InfluxDB.Client.Core;

namespace LifeMonitor.Tracking.Applications;

[Measurement("active_app")]
public class TrackedApplication
{
    [Column("machine", IsTag = true)] public string? Machine { get; set; }
    [Column("process_name", IsTag = true)] public string? ProcessName { get; set; }
    [Column("window_title")] public string? WindowTitle { get; set; }
    [Column(IsTimestamp = true)] public DateTime Timestamp { get; set; }
}
