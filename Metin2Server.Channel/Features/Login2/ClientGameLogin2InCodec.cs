using System.Buffers.Binary;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Channel.Features.Login2;

public class ClientGameLogin2InCodec : IPacketInCodec<ClientGameLogin2Packet>
{
    public ClientGameLogin2Packet Read(ReadOnlySpan<byte> payload)
    {
        var loginBytesLen = Constants.LoginMaxLength + 1;
        
        var off = 0;

        var loginChars = new char[loginBytesLen];
        {
            var slice = payload.Slice(off, loginBytesLen);
            
            for (var i = 0; i < loginBytesLen; i++)
            {
                loginChars[i] = (char)slice[i];
            }
            
            off += loginBytesLen;
        }

        var loginKey = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(off, 4));
        off += 4;

        var adwClientKey = new uint[4];
        for (var i = 0; i < 4; i++)
        {
            adwClientKey[i] = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(off, 4));
            
            off += 4;
        }

        return new ClientGameLogin2Packet(loginChars, loginKey, adwClientKey);
    }
}