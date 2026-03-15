using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using Encrytext.Core.Enums;

namespace Encrytext.Core.Entity;

public class MessageProfile
{
    public string PartnerName { get; set; }
    public Guid PartnerGuid { get; set; }
    public IPEndPoint? PartnerEndPoint { get; set; }
    public byte[] PrivateKey  { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] PartnerPublicKey { get; set; }
    public PartnerStatus Status { get; set; }
    
    public NetworkStream? ActiveStream { get; set; }

    public ObservableCollection<MessageHistory> MessageHistory { get; set; } = [];
    
    
    public override string ToString()
    {
        return $"{PartnerName}   ---   {PartnerEndPoint}";
    }
}