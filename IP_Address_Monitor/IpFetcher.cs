using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace IP_Address_Monitor
{
    internal class IpFetcher
    {
        public static async Task<string> GetPublicIpAsync()
        {
            using var client = new HttpClient();
            try
            {
                // This service returns your public IP as plain text
                string ip = await client.GetStringAsync("https://icanhazip.com/");
                return ip.Trim();
            }
            catch (HttpRequestException)
            {
                return "Unable to retrieve IP. Check your internet connection.";
            }
        }

        private static async Task Main()
        {
            string publicIp = await GetPublicIpAsync();
            Console.WriteLine($"Public IP: {publicIp}");
        }

    }
}