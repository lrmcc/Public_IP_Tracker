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
        static async Task<string> GetExternalIpAsync()
    {
        using var client = new HttpClient();
        try
        {
            // This service returns your public IP as plain text
            string ip = await client.GetStringAsync("https://api.ipify.org");
            return ip.Trim();
        }
        catch (HttpRequestException)
        {
            return "Unable to retrieve IP. Check your internet connection.";
        }
    }
        static async Task Main()
    {
        string externalIp = await GetExternalIpAsync();
        Console.WriteLine($"External IP: {externalIp}");
    }

    }
}