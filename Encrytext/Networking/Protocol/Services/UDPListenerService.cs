using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Encrytext.Core.Entity;
using Encrytext.Core.Enums;
using Encrytext.Networking.Protocol.interfaces;

namespace Encrytext.Networking.Protocol.Services;

public class UdpListenerService
{
    private CancellationTokenSource _cancellationTokenSource;

    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _ = Task.Run(() => UdpListener(_cancellationTokenSource.Token));
    }
    
    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }
    
    
    private async Task UdpListener(CancellationToken cancellationToken)
    {
        try
        {
            using var udp = new UdpClient(63333);
      
            while (!cancellationToken.IsCancellationRequested)
            {
                UdpReceiveResult result = await udp.ReceiveAsync(cancellationToken);
                string json = Encoding.UTF8.GetString(result.Buffer);
                var packet = JsonSerializer.Deserialize<UDPDiscover>(json);
                if (AppState.CurrentUser.SentDiscoveries.Any(c => c.RequestId.Equals(packet!.RequestId)))
                {
                    continue;
                }
                
                switch (packet?.Type)
                {
                    case TypeEnum.request:
                        await HandleDiscoveryRequest(packet, result.RemoteEndPoint, udp );
                        break;
                    case TypeEnum.response: 
                        var correctPackage = JsonSerializer.Deserialize<UDPResponse>(json);
                        await HandleDiscoveryResponse(correctPackage!, result.RemoteEndPoint, udp);
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation canceled");
        }
    }

    private async Task HandleDiscoveryRequest(UDPDiscover packet, IPEndPoint remoteEndPoint, UdpClient udp)
    {

        if (AppState.CurrentUser!.Contacts.Any(c => remoteEndPoint.Equals(c.PartnerEndPoint) && packet.SenderUserId == c.PartnerGuid))
        {
            return;
        }
        
        UDPResponse response = new UDPResponse
        {
            RequestId = packet.RequestId,
            TcpPort = "5555",
            Type = TypeEnum.response,
            UserId = AppState.CurrentUser.Guid,
            UserName = AppState.CurrentUser.Name
        };

        MessageProfile partnerProfile = new MessageProfile
        {
            PartnerEndPoint = remoteEndPoint,
            PartnerName = packet.SenderUserName,
            PartnerGuid = packet.SenderUserId,
            Status = PartnerStatus.Discovered
        };
        
        AppState.CurrentUser.Contacts.Add(partnerProfile);

        byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
        await udp.SendAsync(data, data.Length, remoteEndPoint);
    }
    
    
    private async Task HandleDiscoveryResponse(UDPResponse packet, IPEndPoint remoteEndPoint, UdpClient udp)
    {
        if (AppState.CurrentUser!.SentDiscoveries.Any(d => d.RequestId == packet.RequestId))
        {
            return;
        }
        
        
        UDPResponse response = new UDPResponse
        {
            RequestId = packet.RequestId,
            TcpPort = "5555",
            Type = TypeEnum.response,
            UserId = AppState.CurrentUser.Guid,
            UserName = AppState.CurrentUser.Name
        };
        
        byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
        await udp.SendAsync(data, data.Length, remoteEndPoint);
    }
    
}