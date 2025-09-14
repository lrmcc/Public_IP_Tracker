using IpMonitor.Worker.Core;
using Microsoft.Extensions.Logging;
using System.CommandLine;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// Since the CLI is a short-lived app, we can manually create the dependencies
// that the DI container would normally provide.
using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<IpMonitorService>();
var httpClient = new HttpClient();
var ipMonitorService = new IpMonitorService(httpClient, logger);

var rootCommand = new RootCommand("IP Address Monitoring Tool");

var viewCommand = new Command("view", "View the IP address history.");
viewCommand.SetHandler(async () =>
{
    var entries = await ipMonitorService.ReadLogFileAsync();
    if (entries.Any())
    {
        Console.WriteLine("IP Address History:");
        foreach (var entry in entries)
        {
            var duration = entry.Duration.HasValue ? entry.Duration.Value.ToString(@"d'd 'h'h 'm'm 's's'") : "Current";
            Console.WriteLine($"- IP: {entry.IpAddress}, Start: {entry.StartTimeUtc.ToLocalTime()}, End: {entry.EndTimeUtc?.ToLocalTime()}, Duration: {duration}");
        }
    }
    else
    {
        Console.WriteLine("No IP address history found.");
    }
});


var checkCommand = new Command("check", "Manually check the current IP and log if it has changed.");
checkCommand.SetHandler(async () =>
{
    await ipMonitorService.CheckAndLogIpAddressAsync();
    Console.WriteLine("IP address check complete.");
});


rootCommand.AddCommand(viewCommand);
rootCommand.AddCommand(checkCommand);

return await rootCommand.InvokeAsync(args);
