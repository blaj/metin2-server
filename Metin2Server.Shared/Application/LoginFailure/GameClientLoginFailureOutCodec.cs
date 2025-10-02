using Metin2Server.Shared.Common;
using Metin2Server.Shared.Protocol.Codecs;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Shared.Application.LoginFailure;

public class GameClientLoginFailureOutCodec : IPacketOutCodec<GameClientLoginFailurePacket>
{
    public ReadOnlyMemory<byte> Write(GameClientLoginFailurePacket gameClientHandshakePacket)
    {
        var normalized =
            StringUtils.NormalizeString(gameClientHandshakePacket.Status, Constants.LoginStatusMaxLength + 1);

        var buffer = new byte[normalized.Length];
        for (var i = 0; i < normalized.Length; i++)
        {
            buffer[i] = (byte)normalized[i];
        }

        return buffer;
    }
}