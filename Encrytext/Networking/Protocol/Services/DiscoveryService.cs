using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Encrytext.Networking.Protocol.interfaces;

namespace Encrytext.Networking.Protocol.Services;

public class DiscoveryService
{
    
    private CancellationTokenSource _cancellationTokenSource;

    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _ = Task.Run(() => DiscoveryServiceAsync(_cancellationTokenSource.Token));
    }
    
    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }
    
    private async Task DiscoveryServiceAsync(CancellationToken cancellationToken)
    {
        using var udp = new UdpClient();
        udp.EnableBroadcast = true;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var packet = new UDPDiscover
            {
                RequestId = Guid.NewGuid(),
                SenderUserId = AppState.CurrentUser.Guid
            };

            AppState.CurrentUser.SentDiscoveries.Add(packet);

            byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(packet));
            
            await udp.SendAsync(data, data.Length, new IPEndPoint(IPAddress.Broadcast, 1234));
            
            await Task.Delay(1000, cancellationToken);
        }
      
    }


}