using System.Net.Sockets;
using System.Text;
using Encrytext.Core.Entity;
using Sodium;

namespace Encrytext.Networking.interfaces;

public class MessageSender
{
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