namespace LifeMonitor;

public class WorkerSettings
{
    public int TrackingInterval { get; set; }
    public string InfluxDbUrl { get; set; }
    public string InfluxDbToken { get; set; }
    public string InfluxDbOrg { get; set; }
    public string InfluxDbBucket { get; set; }
}