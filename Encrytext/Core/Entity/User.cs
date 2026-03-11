using System.Collections.ObjectModel;
using Encrytext.Networking.Protocol.interfaces;

namespace Encrytext.Core.Entity;

public class User
{
    public string Name { get; set; }
    public Guid Guid { get; set; }
    public string IpAddress { get; set; }
    public ObservableCollection<MessageProfile> Contacts { get; set; } = [];
    public List<UDPDiscover> SentDiscoveries { get; set; } = [];
    
    public MessageProfile? CurrentMessageProfile { get; set; }

    public override string ToString()
    {
        return $"{Name}   ---   {IpAddress}";
    }
}