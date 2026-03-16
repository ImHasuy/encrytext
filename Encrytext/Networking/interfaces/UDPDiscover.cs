namespace Encrytext.Networking.interfaces;

public class UDPDiscover
{
    public string AppId { get; set; } = "Encrytext";
    public Guid RequestId { get; set; }
    public string SenderUserName { get; set; }
    public Guid SenderUserId { get; set; }
    public TypeEnum Type { get; set; } = TypeEnum.request;
}