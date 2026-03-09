namespace Encrytext.Networking.Protocol.interfaces;

public class UDPDiscover
{
    public string AppId { get; set; } = "Encrytext";
    public Guid RequestId { get; set; }
    public Guid SenderUserId { get; set; }
}