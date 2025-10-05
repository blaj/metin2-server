using System.Collections.Concurrent;

namespace Metin2Server.Channel.Services;

public class CharacterVidAllocator
{
    private readonly byte _channelId;
    private readonly ConcurrentQueue<uint> _free = new();
    private int _counter;

    public CharacterVidAllocator(byte channelId, int seed = 1)
    {
        _channelId = channelId;
        _counter = seed & 0x00FF_FFFF;
    }

    public uint Allocate()
    {
        if (_free.TryDequeue(out var reused))
        {
            return reused;
        }

        var next = (uint)(Interlocked.Increment(ref _counter) & 0x00FF_FFFF);
        if (next == 0)
        {
            next = 1;
        }
        
        return ((uint)_channelId << 24) | next;
    }

    public void Release(uint vid)
    {
        _free.Enqueue(vid);
    }
}