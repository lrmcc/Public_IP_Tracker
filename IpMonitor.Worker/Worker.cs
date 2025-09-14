namespace IP_Address_Monitor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval;

        // Inject IServiceProvider to create a new scope for each run.
        // Inject IConfiguration to get settings from appsettings.json
        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            // Read check interval from configuration, with a fallback default.
            var checkIntervalSeconds = configuration.GetValue<int>("WorkerSettings:CheckIntervalSeconds", 30);
            _checkInterval = TimeSpan.FromSeconds(checkIntervalSeconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("IP Monitor Worker starting up.");
            _logger.LogInformation("IP check interval is set to {Seconds} seconds.", _checkInterval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    _logger.LogInformation("IP address is: {ip}", await IpFetcher.GetPublicIpAsync());

                    // Create a new scope to resolve scoped services like IpMonitorService
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var ipMonitorService = scope.ServiceProvider.GetRequiredService<IpMonitorService>();
                        await ipMonitorService.CheckAndLogIpAddressAsync();
                    }
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
