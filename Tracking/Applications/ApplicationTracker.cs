using System.Diagnostics;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Options;

namespace LifeMonitor.Tracking.Applications;

public class ApplicationTracker(ILogger<ApplicationTracker> logger, InfluxDBClient client, IOptions<WorkerSettings> settings)
{
    private static TrackedApplication GetCurrentApplication()
    {
        ActiveWindowModel activeWindow = WindowApi.GetActiveWindowTitle() ?? ActiveWindowModel.CreateEmpty();
        Process? process = Process.GetProcesses().FirstOrDefault(p => p.MainWindowHandle == activeWindow.WindowHandle);
            
        return new TrackedApplication
        {
            ProcessName = process?.ProcessName ?? "Unknown",
            Machine = Environment.MachineName,
            WindowTitle = process?.MainWindowTitle ?? "Unknown",
            Timestamp = DateTime.UtcNow
        };
    }

    public void TrackCurrentApplication()
    {
        try
        {
            TrackedApplication app = GetCurrentApplication();
            
            logger.LogInformation("{Timestamp} Sending current application to InfluxDB: {ProcessName} @ {MachineName}", app.Timestamp, app.ProcessName, app.Machine);
            
            using WriteApi? writeApi = client.GetWriteApi();
            writeApi.WriteMeasurement(app, WritePrecision.Ns, "test-bucket", "test-org");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error tracking application");
        }
    }
}