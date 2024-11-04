using InfluxDB.Client;
using InfluxDB.Client.Writes;
using System.Diagnostics;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Options;

namespace LifeMonitor
{
    public class Worker(ILogger<Worker> logger, IOptions<WorkerSettings> settings) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Connecting to InfluxDB @ {InfluxDbUrl}", settings.Value.InfluxDbUrl);
            using InfluxDBClient client = new(settings.Value.InfluxDbUrl.Trim(), settings.Value.Token);
            
            while (!stoppingToken.IsCancellationRequested)
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
                    writeApi.WritePoint(point, settings.Value.Bucket, settings.Value.Org);

                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Tracked application: {Application} at {Time}", currentApplication, DateTimeOffset.Now);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error tracking application");
                }

                await Task.Delay(settings.Value.TrackingInterval, stoppingToken);
            }
        }

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
    }
}

internal record TrackedApplication
{
    public string? ProcessName { get; set; }
    public string? MachineName { get; set; }
    public string? WindowTitle { get; set; }
    public DateTime Timestamp { get; set; }
}