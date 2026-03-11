using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using Encrytext.Core.Entity;
using Sodium;

namespace Encrytext.Networking.Protocol.Services;

public class HandleClient
{
    public async Task HandleClientAsync(TcpClient client, Action onDisconnect)
    {
        try
        {
            await using var stream = client.GetStream();
           
            NegotiateResult IpDetailes = await NegotiateAsync(stream);
            var contact = AppState.CurrentUser?.Contacts?.FirstOrDefault(c => c.PartnerEndPoint!.Equals(client.Client.RemoteEndPoint));
            
            AppState.CurrentUser!.CurrentMessageProfile = contact;
            
            contact!.ActiveStream = stream;
            
            contact!.PublicKey = IpDetailes.publicKey;
            contact.PrivateKey = IpDetailes.privateKey;
            contact.PartnerPublicKey = IpDetailes.PartnerPublicKey;
             
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

        var negotiationResult = new NegotiateResult
        {
            privateKey = keypair.PrivateKey,
            publicKey = keypair.PublicKey
        };
        
       byte[] publicKeyBytes = negotiationResult.publicKey;
       byte[] lenghtInBytes = BitConverter.GetBytes(publicKeyBytes.Length); // Bc its a number it will be 4 bytes
        
       await stream.WriteAsync(lenghtInBytes,0,lenghtInBytes.Length);
       await stream.WriteAsync(publicKeyBytes,0,publicKeyBytes.Length);
       await stream.FlushAsync();
       
       
       byte[] partnerKeyLenghtBuffer = new byte[4];
       await ReadFromStreamAsync(stream, partnerKeyLenghtBuffer);
       
       int partnerKeyLenghtToInt = BitConverter.ToInt32(partnerKeyLenghtBuffer,0);
       
       byte[] partnerPublicKeyBuffer = new byte[partnerKeyLenghtToInt];
       await ReadFromStreamAsync(stream, partnerPublicKeyBuffer);
       
       negotiationResult.PartnerPublicKey = partnerPublicKeyBuffer;
       
        return negotiationResult;
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
                byte[] decryptedPacket = SealedPublicKeyBox.Open(packet, messageProfile.PrivateKey, messageProfile.PublicKey);
            
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

    public async Task SendMessageAsync(NetworkStream stream, string message, MessageProfile messageProfile)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);

        byte[] encryptedBuffer = SealedPublicKeyBox.Create(buffer, messageProfile.PartnerPublicKey);
        
        byte[] bufferLenght = BitConverter.GetBytes(encryptedBuffer.Length);
        
        //send lenght
        await stream.WriteAsync(bufferLenght,0,bufferLenght.Length);
        
        //send encrypted message
        
        await stream.WriteAsync(encryptedBuffer,0,encryptedBuffer.Length);
        
        await stream.FlushAsync();
    }
    
}
