using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Encrytext.Core.Entity;
using Encrytext.Core.Enums;
using Sodium;

namespace Encrytext.Networking.Protocol.Services;

public class HandleClient
{
    public async Task HandleClientAsync(TcpClient client, Action onDisconnect)
    {
        try
        {
            //set tcpClient
            AppState.CurrentUser.currenConnectedClient = client;
            
            
            await using var stream = client.GetStream();
            NegotiateResult IpDetailes = await NegotiateAsync(stream);
            var contact = AppState.CurrentUser?.Contacts?.FirstOrDefault(c => c.PartnerEndPoint.Address.Equals((IPEndPoint)client.Client.RemoteEndPoint));
            
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
/*
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
*/


    public async Task<NegotiateResult> NegotiateAsync(NetworkStream stream)
    {
        var keypair = PublicKeyBox.GenerateKeyPair();
        byte[] myKey = keypair.PublicKey;
        byte[] myLength = BitConverter.GetBytes(myKey.Length);

        // 1. Kick off the Read Task but DO NOT 'await' it yet.
        // This tells the OS: "I am ready to receive whenever the data arrives."
        byte[] partnerLenBuf = new byte[4];
        Task readLenTask = ReadFromStreamAsync(stream, partnerLenBuf);

        // 2. Now perform the Write.
        await stream.WriteAsync(myLength, 0, 4);
        await stream.WriteAsync(myKey, 0, myKey.Length);
        await stream.FlushAsync();

        // 3. Now await the reading task that we started in step 1.
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
