using System.Buffers.Binary;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Metin2Server.Shared.Protocol;

namespace Metin2Server.Network;

public class PacketFramer
{
    private const int MaxFrameSize = 64 * 1024;
    private const int HeaderBytesWithSize = 3;

    public static async IAsyncEnumerable<Packet> ReadPacketsAsync(
        Socket socket,
        PacketRuleRegistryIn packetRuleRegistryIn,
        Session session,
        PacketSequencer packetSequencer,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var headerBuffer = new byte[1];

        while (!cancellationToken.IsCancellationRequested)
        {
            await ReadExactlyAsync(socket, headerBuffer, cancellationToken);
            var header = (ClientGameHeader)headerBuffer[0];

            if (!packetRuleRegistryIn.TryGetRule(header, out var rule))
            {
                throw new IOException($"Unknown incoming header: {header}");
            }

            int payloadLen = rule.PacketSizeKind switch
            {
                PacketSizeKind.TotalSize => await ReadLenAndCalcPayloadAsync(socket, cancellationToken),
                PacketSizeKind.NoneFixed => rule.ExactPayloadSize
                                            ?? throw new IOException($"Header={header} SizeKind=NoneFixed requires ExactPayloadSize"),
                _ => throw new NotSupportedException($"Unsupported SizeKind for header=0x{header:X2}")
            };

            var expectSeq = rule.SequenceBehavior == SequenceBehavior.ExpectInbound;
            var raw = new byte[payloadLen + (expectSeq ? 1 : 0)];
            if (raw.Length > 0)
            {
                await ReadExactlyAsync(socket, raw, cancellationToken);
            }

            ReadOnlyMemory<byte> payloadMem;
            if (expectSeq)
            {
                var seq = raw[^1];
                if (!packetSequencer.ConsumeInbound(seq, session))
                {
                    throw new IOException($"Sequence mismatch for header={header}: got {seq:X2}");
                }

                payloadMem = raw.AsMemory(0, Math.Max(0, raw.Length - 1));
            }
            else
            {
                payloadMem = raw.AsMemory();
            }

            if (!packetRuleRegistryIn.Validate(header, payloadMem.Length))
            {
                throw new IOException($"Payload length invalid for header={header}");
            }

            yield return new Packet((byte)header, payloadMem.IsEmpty ? null : payloadMem.ToArray());
        }
    }

    public static async Task WriteFrameAsync(
        Socket socket,
        PacketRuleRegistryOut registryOut,
        GameClientHeader header,
        ReadOnlyMemory<byte> payload,
        Session session,
        PacketSequencer packetSequencer,
        CancellationToken cancellationToken)
    {
        if (!registryOut.TryGetRule(header, out var rule))
        {
            throw new IOException($"Unknown header={header}");
        }
        
        ReadOnlyMemory<byte> finalPayload = payload;
        if ((rule.SequenceBehavior & SequenceBehavior.PrependOutbound) != 0)
        {
            var b = packetSequencer.NextOutbound(session);
            var buf = new byte[payload.Length + 1];
            buf[0] = b;
            if (!payload.IsEmpty)
            {
                payload.CopyTo(buf.AsMemory(1));
            }
            finalPayload = buf;
        }

        switch (rule.PacketSizeKind)
        {
            case PacketSizeKind.TotalSize:
            {
                checked
                {
                    var total = (ushort)(HeaderBytesWithSize + finalPayload.Length);
                    if (total > MaxFrameSize)
                    {
                        throw new IOException($"Frame too large: {total} > {MaxFrameSize}");
                    }

                    var buf = new byte[total];
                    buf[0] = (byte)header;
                    BinaryPrimitives.WriteUInt16LittleEndian(buf.AsSpan(1, 2), total);
                    if (!finalPayload.IsEmpty)
                    {
                        finalPayload.CopyTo(buf.AsMemory(HeaderBytesWithSize));
                    }

                    await SendAllAsync(socket, buf, cancellationToken);
                }
                break;
            }

            case PacketSizeKind.NoneFixed:
            {
                var expected = rule.ExactPayloadSize ?? 0;
                if (finalPayload.Length != expected)
                {
                    throw new IOException($"Header={header} expects payload {expected}B, got {finalPayload.Length}B");
                }

                var buf = new byte[1 + finalPayload.Length];
                buf[0] = (byte)header;
                if (!finalPayload.IsEmpty)
                {
                    finalPayload.CopyTo(buf.AsMemory(1));
                }

                await SendAllAsync(socket, buf, cancellationToken);
                break;
            }

            default:
            {
                throw new NotSupportedException($"Unsupported SizeKind for header=0x{header:X2}");
            }
        }
    }

    private static async Task ReadExactlyAsync(Socket socket, byte[] buffer, CancellationToken cancellationToken)
    {
        var off = 0;

        while (off < buffer.Length)
        {
            var n = await socket.ReceiveAsync(
                buffer.AsMemory(off, buffer.Length - off), 
                SocketFlags.None,
                cancellationToken);

            if (n == 0)
            {
                throw new IOException("Remote closed");
            }

            off += n;
        }
    }

    private static async Task SendAllAsync(Socket socket, byte[] buffer, CancellationToken cancellationToken)
    {
        var off = 0;

        while (off < buffer.Length)
        {
            var n = await socket.SendAsync(
                buffer.AsMemory(off, buffer.Length - off),
                SocketFlags.None,
                cancellationToken);

            if (n == 0)
            {
                throw new IOException("Remote closed during send");
            }

            off += n;
        }
    }
    
    private static async Task<int> ReadLenAndCalcPayloadAsync(Socket socket, CancellationToken ct)
    {
        var sb = new byte[2];
        await ReadExactlyAsync(socket, sb, ct);

        var total = BinaryPrimitives.ReadUInt16LittleEndian(sb);
        
        if (total < HeaderBytesWithSize)
        {
            throw new IOException($"Bad frame size={total}");
        }
        
        if (total > MaxFrameSize)
        {
            throw new IOException($"Frame too large: {total} > {MaxFrameSize}");
        }

        return total - HeaderBytesWithSize;
    }
}