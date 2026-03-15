using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;

namespace Encrytext.Networking.Protocol.Services;

public class TcpListenerService
{
    public async Task TcpListenerServiceAsync()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 5555);
        listener.Start();
        
        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            if (AppState.CurrentUser.currenConnectedClient.Connected ||
                AppState.CurrentUser?.UserChosenMessageProfile?.PartnerGuid == AppState.CurrentUser?.CurrentMessageProfile?.PartnerGuid)
            {
                client.Close();
                continue;
            }
            AppState.CurrentUser.currenConnectedClient = client;
            
            _ = Task.Run(async () => await new HandleClient().HandleClientAsync(client, () => {AppState.CurrentUser.currenConnectedClient  = null;}));
        }
    }
}

