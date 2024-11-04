using LifeMonitor;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<WorkerSettings>(builder.Configuration.GetSection("WorkerSettings"));
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();