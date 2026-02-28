namespace Encrytext.Core.Entity;

public class User
{
    public string Name { get; set; }
    public Guid Guid { get; set; }
    public string IpAddress { get; set; }
    public List<MessageProfile> Contacts { get; set; }
    
}