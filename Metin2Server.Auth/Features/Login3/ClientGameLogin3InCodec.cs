using System.Buffers.Binary;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.Protocol.Codecs;

namespace Metin2Server.Auth.Features.Login3;

public class ClientGameLogin3InCodec : IPacketInCodec<ClientGameLogin3Packet>
{
    public ClientGameLogin3Packet Read(ReadOnlySpan<byte> payload)
    {
        var loginBytesLength = Constants.LoginMaxLength + 1;
        var passwordBytesLength = Constants.PasswordMaxLength + 1;

        var off = 0;

        var loginChars = new char[loginBytesLength];
        {
            var slice = payload.Slice(off, loginBytesLength);
            
            for (var i = 0; i < loginBytesLength; i++)
            {
                loginChars[i] = (char)slice[i];
            }
            
            off += loginBytesLength;
        }

        var passwordChars = new char[passwordBytesLength];
        {
            var slice = payload.Slice(off, passwordBytesLength);
            
            for (var i = 0; i < passwordBytesLength; i++)
            {
                passwordChars[i] = (char)slice[i];
            }
            
            off += passwordBytesLength;
        }

        var key = new uint[4];
        
        for (var i = 0; i < 4; i++)
        {
            key[i] = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(off, 4));
            
            off += 4;
        }

        return new ClientGameLogin3Packet(loginChars, passwordChars, key);
    }
}