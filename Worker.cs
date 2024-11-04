using LifeMonitor.Tracking.Applications;
using Microsoft.Extensions.Options;

namespace LifeMonitor;

public class Worker(ApplicationTracker appTracker, IOptions<WorkerSettings> settings, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker starting");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            appTracker.TrackCurrentApplication();
            
            await Task.Delay(settings.Value.TrackingInterval, stoppingToken);
        }
        
        logger.LogInformation("Worker stopping");
        stoppingToken.ThrowIfCancellationRequested();
    }
}