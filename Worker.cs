using LifeMonitor.Tracking.Applications;
using Microsoft.Extensions.Options;

namespace LifeMonitor;

public class Worker(ApplicationTracker appTracker, IOptions<WorkerSettings> settings) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            appTracker.TrackCurrentApplication();
                
            await Task.Delay(settings.Value.TrackingInterval, stoppingToken);
        }
    }
}