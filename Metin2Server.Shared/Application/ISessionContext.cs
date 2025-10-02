using Metin2Server.Shared.Encryption;
using Metin2Server.Shared.Protocol;

namespace Metin2Server.Shared.Application;

public interface ISessionContext
{
    long Id { get; }
    SessionPhase Phase { get; set; }
    DateTime LastActivityUtc { get; }
    SessionState State { get; }
    uint? LoginKey { get; set; }
    long? AccountId { get; set; }
    public int SequenceIndex { get; set; }
}