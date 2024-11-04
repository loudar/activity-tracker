using InfluxDB.Client;
using LifeMonitor;
using LifeMonitor.Tracking.Applications;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<WorkerSettings>(builder.Configuration.GetSection("WorkerSettings"));
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ApplicationTracker>();
builder.Services.AddSingleton<InfluxDBClient>(_ =>
{
    IConfigurationSection config = builder.Configuration.GetSection("WorkerSettings");
    return new InfluxDBClient(config["InfluxDbUrl"], config["InfluxDbToken"]);
});

IHost host = builder.Build();
host.Run();