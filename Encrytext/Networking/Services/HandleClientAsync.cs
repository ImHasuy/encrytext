using System.Net;
using System.Net.Sockets;
using System.Text;
using Encrytext.Core.Entity;
using Encrytext.Core.Enums;
using Encrytext.Networking.interfaces;
using Sodium;

namespace Encrytext.Networking.Services;

public class HandleClient
{
    public async Task HandleClientAsync(TcpClient client, Action onDisconnect)
    {
        try
        {
            await using var stream = client.GetStream();
            NegotiateResult IpDetailes = await NegotiateAsync(stream);

            var tcpIp = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.MapToIPv4();

            var contact = AppState.CurrentUser?.Contacts?
                .FirstOrDefault(c =>
                    c.PartnerEndPoint.Address.MapToIPv4().Equals(tcpIp));
            
            //Setting MessageProfile and stream for the current user
            contact!.Status = PartnerStatus.Connected;
            contact!.ActiveStream = stream;
            
            contact!.PublicKey = IpDetailes.publicKey;
            contact.PrivateKey = IpDetailes.privateKey;
            contact.PartnerPublicKey = IpDetailes.PartnerPublicKey;
            contact.MessageHistory = []; 
            
            AppState.CurrentUser!.CurrentMessageProfile = contact;
            
            await MessageListeningLoopAsync(stream, contact);
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Console.WriteLine("Client disconnected");
            client.Close();
            onDisconnect();
        }
    }


    public async Task<NegotiateResult> NegotiateAsync(NetworkStream stream)
    {
        var keypair = PublicKeyBox.GenerateKeyPair();
        byte[] myKey = keypair.PublicKey;
        byte[] myLength = BitConverter.GetBytes(myKey.Length);

      
        byte[] partnerLenBuf = new byte[4];
        Task readLenTask = ReadFromStreamAsync(stream, partnerLenBuf);

      
        await stream.WriteAsync(myLength, 0, 4);
        await stream.WriteAsync(myKey, 0, myKey.Length);
        await stream.FlushAsync();
        
        await readLenTask; 
    
        int partnerKeyLen = BitConverter.ToInt32(partnerLenBuf, 0);
        byte[] partnerKeyBuf = new byte[partnerKeyLen];
        await ReadFromStreamAsync(stream, partnerKeyBuf);

        return new NegotiateResult {
            privateKey = keypair.PrivateKey,
            publicKey = keypair.PublicKey,
            PartnerPublicKey = partnerKeyBuf
        };
    }
    public async Task ReadFromStreamAsync(NetworkStream stream, byte[] buffer)
    {
        int readCount = 0;

        while (readCount < buffer.Length)
        {
            int readed = await stream.ReadAsync(buffer,readCount,buffer.Length-readCount);
            if (readed == 0) throw new Exception("Connection closed prematurely");
            readCount += readed;
        }

    }


    public async Task MessageListeningLoopAsync(NetworkStream stream, MessageProfile messageProfile)
    {
        try
        {
            while (true)
            {
                // Reading the lenght of the buffer
                byte[] bufferLenght = new byte[4];
                await ReadFromStreamAsync(stream, bufferLenght);
                int bufferSize = BitConverter.ToInt32(bufferLenght,0);
            
                // Reading the message
                byte[] packet = new byte[bufferSize];
                await ReadFromStreamAsync(stream, packet);
            
                //decypt 
                byte[] decryptedPacket = SealedPublicKeyBox.Open(packet,messageProfile.PrivateKey, messageProfile.PublicKey);
            
                string message = Encoding.UTF8.GetString(decryptedPacket);

                var historyElement = new MessageHistory
                {
                    Sendername = messageProfile.PartnerName,
                    Message = message,
                    TimeStamp = DateTime.Now
                };
                
                
                
                messageProfile.MessageHistory.Add(historyElement);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
}
