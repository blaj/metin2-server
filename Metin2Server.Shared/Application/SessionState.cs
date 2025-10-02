namespace Metin2Server.Shared.Application;

public class SessionState
{
    public uint? HandshakeCrc { get; set; }
    public uint ClientTime { get; set; }
    public bool IsHandshaking { get; set; }
    public int HandshakeRetryCount { get; set; }
    public uint HandshakeSentTime { get; set; }
}