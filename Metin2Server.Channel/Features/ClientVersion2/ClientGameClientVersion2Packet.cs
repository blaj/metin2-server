using Metin2Server.Shared.Common;

namespace Metin2Server.Channel.Features.ClientVersion2;

public record ClientGameClientVersion2Packet(char[] Filename, char[] Timestamp)
{
    public static int Size() =>
        (sizeof(byte) * (Constants.ClientVersion2Length + 1)) +
        (sizeof(byte) * (Constants.ClientVersion2Length + 1));
}