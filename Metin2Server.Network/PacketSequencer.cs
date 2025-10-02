using Metin2Server.Shared.Protocol;

namespace Metin2Server.Network;

public class PacketSequencer
{
    public bool ConsumeInbound(byte actual, Session session)
    {
        var expected = SequenceTable.Sequences[session.SequenceIndex];
        
        if (actual != expected)
        {
            return false;
        }
        
        session.SequenceIndex = (session.SequenceIndex + 1) % SequenceTable.Sequences.Length;
        
        return true;
    }

    public byte NextOutbound(Session session)
    {
        var nextOutbound = SequenceTable.Sequences[session.SequenceIndex];
        
        session.SequenceIndex = (session.SequenceIndex + 1) % SequenceTable.Sequences.Length;
        
        return nextOutbound;
    }

    public void Reset(Session session) => session.SequenceIndex = 0;
}