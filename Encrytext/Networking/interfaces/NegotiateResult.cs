namespace Encrytext.Networking.interfaces;

public class NegotiateResult
{
    public byte[] publicKey { get; set; }
    public byte[] privateKey { get; set; }
    public byte[] PartnerPublicKey { get; set; }
}