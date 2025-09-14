using IP_Address_Monitor;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();


builder.Services.AddSingleton<IpMonitor>();
builder.Services.AddSingleton<NotificationService>();

builder.Services.Configure<IpMonitorOptions>(
    builder.Configuration.GetSection("IpMonitor"));


var host = builder.Build();

host.Run();