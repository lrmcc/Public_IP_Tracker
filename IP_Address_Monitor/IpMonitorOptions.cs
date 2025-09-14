using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_Address_Monitor
{
    internal class IpMonitorOptions
    {

        /// <summary>
        /// The monitor interval in seconds.
        /// </summary>
        int CheckIntervalSeconds { get; set; }

        /// <summary>
        /// Notify on IP change.
        /// </summary>
        bool NotifyOnIpChange { get; set; }

    }
}
