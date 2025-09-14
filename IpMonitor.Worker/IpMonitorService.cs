using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IP_Address_Monitor
{
    internal class IpMonitorService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IpMonitorService> _logger;
        private readonly string _logFilePath;

        // The constructor now accepts dependencies, which will be provided by the host's service container.
        public IpMonitorService(HttpClient httpClient, ILogger<IpMonitorService> logger, string? logFilePath = null)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Store log files in a dedicated directory within the user's profile
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var logDirectory = Path.Combine(appDataPath, "IpMonitor");
            Directory.CreateDirectory(logDirectory);
            _logFilePath = logFilePath ?? Path.Combine(logDirectory, "ip_log.json");
        }

        /// <summary>
        /// Checks the current external IP address and updates the log if it has changed.
        /// </summary>
        public async Task CheckAndLogIpAddressAsync()
        {
            try
            {
                var currentIp = await GetExternalIpAddressAsync();
                if (string.IsNullOrEmpty(currentIp))
                {
                    _logger.LogWarning("Could not retrieve external IP address.");
                    return;
                }

                var logEntries = await ReadLogFileAsync();
                var lastEntry = logEntries.LastOrDefault();

                if (lastEntry == null || lastEntry.IpAddress != currentIp)
                {
                    if (lastEntry != null)
                    {
                        lastEntry.EndTimeUtc = DateTime.UtcNow;
                    }

                    var newEntry = new IpLogEntry
                    {
                        IpAddress = currentIp,
                        StartTimeUtc = DateTime.UtcNow
                    };
                    logEntries.Add(newEntry);
                    _logger.LogInformation("New IP Address Detected: {IpAddress} at {Time}", newEntry.IpAddress, newEntry.StartTimeUtc);
                }
                else
                {
                    _logger.LogInformation("IP address {IpAddress} is unchanged since {StartTime}", lastEntry.IpAddress, lastEntry.StartTimeUtc);
                }

                await WriteLogFileAsync(logEntries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking and logging the IP address.");
            }
        }

        /// <summary>
        /// Retrieves the current external IP address from an external service.
        /// </summary>
        private async Task<string?> GetExternalIpAddressAsync()
        {
            try
            {
                // Using ipify.org as it's simple and returns the IP as plain text.
                return await _httpClient.GetStringAsync("https://api.ipify.org");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Failed to get IP from primary service. Trying fallback.");
                // Fallback service
                try
                {
                    return await _httpClient.GetStringAsync("https://icanhazip.com");
                }
                catch (HttpRequestException fallbackEx)
                {
                    _logger.LogError(fallbackEx, "Failed to get IP from fallback service.");
                    return null;
                }
            }
        }

        /// <summary>
        /// Reads and deserializes the log entries from the JSON file.
        /// </summary>
        public async Task<List<IpLogEntry>> ReadLogFileAsync()
        {
            if (!File.Exists(_logFilePath))
            {
                return new List<IpLogEntry>();
            }

            var json = await File.ReadAllTextAsync(_logFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<IpLogEntry>();
            }

            return JsonSerializer.Deserialize<List<IpLogEntry>>(json) ?? new List<IpLogEntry>();
        }

        /// <summary>
        /// Serializes and writes the log entries to the JSON file.
        /// </summary>
        private async Task WriteLogFileAsync(List<IpLogEntry> logEntries)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(logEntries, options);
            await File.WriteAllTextAsync(_logFilePath, json);
        }
    }
}
