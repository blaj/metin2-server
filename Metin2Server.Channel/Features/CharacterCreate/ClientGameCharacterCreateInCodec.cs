using System.Buffers.Binary;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.CharacterCreate;

public class ClientGameCharacterCreateInCodec : IPacketInCodec<ClientGameCharacterCreatePacket>
{
    public ClientGameCharacterCreatePacket Read(ReadOnlySpan<byte> payload)
    {
        var offset = 0;

        var index = payload[offset++];
        
        var nameLen = Constants.CharacterNameMaxLength + 1;
        var name = new char[nameLen];
        for (var i = 0; i < nameLen; i++)
        {
            name[i] = (char)payload[offset++];
        }

        var job = BinaryPrimitives.ReadUInt16LittleEndian(payload.Slice(offset, 2));
        offset += 2;

        var shape = payload[offset++];

        var con = payload[offset++];
        var intel = payload[offset++];
        var str = payload[offset++];
        var dex = payload[offset++];

        return new ClientGameCharacterCreatePacket(index, name, job, shape, con, intel, str, dex);
    }
}