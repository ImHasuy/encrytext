namespace Encrytext.Networking.interfaces;

public class UDPResponse
{
    public string AppId { get; set; } = "Encrytext";
    public Guid RequestId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string TcpPort { get; set; } 
    public TypeEnum Type { get; set; } = TypeEnum.response;
}