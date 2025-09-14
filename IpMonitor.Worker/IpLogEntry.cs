using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP_Address_Monitor
{
    internal class IpLogEntry
    {
        public string IpAddress { get; set; } = string.Empty;
        public DateTime StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
        public TimeSpan? Duration => EndTimeUtc.HasValue ? EndTimeUtc.Value - StartTimeUtc : null;
    }
}
