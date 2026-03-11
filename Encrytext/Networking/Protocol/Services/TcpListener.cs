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
        TcpClient? currenClient = null;
        
        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            if (currenClient != null && currenClient.Connected)
            {
                client.Close();
                continue;
            }
            currenClient = client;
            
            _ = Task.Run(async () => await new HandleClient().HandleClientAsync(client, () => {currenClient = null;}));
        }
    }
}

