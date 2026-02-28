using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Encrytext.Core.Services;

public class IpAddressService
{
    public string GetIpAddress()
    {
        var ip =
            NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .SelectMany(n => n.GetIPProperties().UnicastAddresses)
                .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(a => a.Address.ToString())
                .FirstOrDefault(ip => ip.StartsWith("10.2."));

        Console.WriteLine(ip);
        return ip ?? "asdasdasd";
    }
} 