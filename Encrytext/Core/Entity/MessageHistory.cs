namespace Encrytext.Core.Entity;

public class MessageHistory
{
    public string Sendername { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }

    public override string ToString()
    {
        return $"{Sendername}: {Message}";
    }
}