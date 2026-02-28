using Encrytext.Core.Enums;

namespace Encrytext.Core.Entity;

public class MessageProfile
{
    public byte[] PrivateKey  { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] PartnerPublicKey { get; set; }
    public PartnerStatus Status { get; set; } 
}