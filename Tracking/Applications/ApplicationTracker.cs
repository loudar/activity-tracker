using System.Diagnostics;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Options;

namespace LifeMonitor.Tracking.Applications;

public class ApplicationTracker(ILogger<ApplicationTracker> logger, InfluxDBClient client, IOptions<WorkerSettings> settings)
{
    private TrackedApplication GetCurrentApplication()
    {
        ActiveWindowModel activeWindow = WindowApi.GetActiveWindowTitle() ?? ActiveWindowModel.CreateEmpty();
        logger.LogInformation("Active window: {WindowTitle}", activeWindow.WindowTitle);
        Process? process = Process.GetProcesses().FirstOrDefault(p => p.MainWindowHandle == activeWindow.WindowHandle);
            
        return new TrackedApplication
        {
            ProcessName = process?.ProcessName ?? "Unknown",
            MachineName = Environment.MachineName,
            WindowTitle = process?.MainWindowTitle ?? "Unknown",
            Timestamp = DateTime.UtcNow
        };
    }

    public void TrackCurrentApplication()
    {
        try
        {
            TrackedApplication currentApplication = GetCurrentApplication();
            PointData? point = PointData
                .Measurement("open_applications")
                .Tag("application", currentApplication.ProcessName)
                .Tag("machine", currentApplication.MachineName)
                .Tag("window_title", currentApplication.WindowTitle)
                .Field("count", 1)
                .Timestamp(currentApplication.Timestamp, WritePrecision.Ns);
                    
            logger.LogInformation("Sending current application to InfluxDB: {ProcessName} @ {MachineName}", currentApplication.ProcessName, currentApplication.MachineName);

            using WriteApi? writeApi = client.GetWriteApi();
            writeApi.WritePoint(point, settings.Value.InfluxDbUrl, settings.Value.InfluxDbBucket);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Tracked application: {Application} at {Time}", currentApplication, DateTimeOffset.Now);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error tracking application");
        }
    }
}