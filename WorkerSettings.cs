namespace LifeMonitor;

public class WorkerSettings
{
    public int TrackingInterval { get; set; }
    public string InfluxDbUrl { get; set; }
    public string Token { get; set; }
    public string Org { get; set; }
    public string Bucket { get; set; }
}