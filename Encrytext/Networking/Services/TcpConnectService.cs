using System.Net.Sockets;

namespace Encrytext.Networking.Services;

public class TcpConnectService
{
    public async Task TcpConnectServiceAsync()
    {
        var client = new TcpClient();

        //Check for existing connection, if exists, then 
        if (AppState.CurrentUser!.CurrentMessageProfile != null && AppState.CurrentUser.CurrentMessageProfile.PartnerName == AppState.CurrentUser.UserChosenMessageProfile.PartnerName)
        {
            return;
        }
        
        await client.ConnectAsync(AppState.CurrentUser!.UserChosenMessageProfile!.PartnerEndPoint!.Address, 5555);

        //set tcpClient
        AppState.CurrentUser.currenConnectedClient = client;
        
        var handler = new HandleClient();
        await handler.HandleClientAsync(client, () => {AppState.CurrentUser.CurrentMessageProfile = null;});

    }
}