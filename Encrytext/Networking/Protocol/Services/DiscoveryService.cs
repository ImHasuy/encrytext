using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Encrytext.Networking.Protocol.interfaces;

namespace Encrytext.Networking.Protocol.Services;

public class DiscoveryServiceC
{

    public async Task DiscoveryService()
    {
        using var udp = new UdpClient();
        udp.EnableBroadcast = true;

        var packet = new UDPDiscover
        {
            RequestId = Guid.NewGuid(),
            SenderUserId = AppState.CurrentUser.Guid
        };

        AppState.CurrentUser.SentDiscoveries.Add(packet);

        byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(packet));

        await udp.SendAsync(data, data.Length, new IPEndPoint(IPAddress.Broadcast, 1234));

    }
}