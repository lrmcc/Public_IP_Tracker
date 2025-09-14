using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_Address_Monitor
{
    internal class IpMonitor
    {
        /// <summary>
        /// String to hold the last known IP address
        /// </summary>
        string? LastKnownIpAddress;

        private readonly IpMonitorOptions _options;
        public IpMonitor(IOptions<IpMonitorOptions> options)
        {
            _options = options.Value;
        }

        public async Task InitializeAsync()
        {
            LastKnownIpAddress = await IpFetcher.GetPublicIpAsync();
        }

    
    }
}
