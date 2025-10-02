using System.Buffers.Binary;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.Protocol.Codecs;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Channel.Features.Common.CharacterCreateSuccess;

public class GameClientCharacterCreateSuccessOutCodec
    : IPacketOutCodec<GameClientCharacterCreateSuccessPacket>
{
    public ReadOnlyMemory<byte> Write(GameClientCharacterCreateSuccessPacket packet)
    {
       const int NAME_LEN  = Constants.CharacterNameMaxLength + 1;
        const int DUMMY_LEN = Constants.DummyLength; // dla TSimplePlayer powinno być 4

        var c = packet.SimpleCharacter;

        var buffer = new byte[GameClientCharacterCreateSuccessPacket.Size()];
        var span = buffer.AsSpan();
        var offset = 0;

        // 1) Index
        span[offset++] = packet.Index;

        // 2) Id
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), c.Id);
        offset += 4;

        // 3) Name (C-string; zakładamy, że c.Name ma długość NAME_LEN i jest z '\0')
        for (int i = 0; i < NAME_LEN; i++)
            span[offset + i] = (byte)c.Name[i];
        offset += NAME_LEN;

        // 4) Job, Level
        span[offset++] = c.Job;
        span[offset++] = c.Level;

        // 5) PlayMinutes
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), c.PlayMinutes);
        offset += 4;

        // 6) ST, HT, DX, IQ
        span[offset++] = c.St;
        span[offset++] = c.Ht;
        span[offset++] = c.Dx;
        span[offset++] = c.Iq;

        // 7) MainPart
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), c.MainPart);
        offset += 2;

        // 8) ChangeName
        span[offset++] = c.ChangeName;

        // 9) HairPart
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), c.HairPart);
        offset += 2;

        // 10) Dummy (dokładnie DUMMY_LEN bajtów)
        c.Dummy.AsSpan(0, DUMMY_LEN).CopyTo(span.Slice(offset, DUMMY_LEN));
        offset += DUMMY_LEN;

        // 11) X, Y, Addr
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), c.X);    offset += 4;
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), c.Y);    offset += 4;
        BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(offset, 4), c.Addr); offset += 4;

        // 12) Port
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), c.Port);
        offset += 2;

        // 13) SkillGroup
        span[offset++] = c.SkillGroup;

        return buffer;
    }

}