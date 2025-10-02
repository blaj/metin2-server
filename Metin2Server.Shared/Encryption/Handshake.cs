using Metin2Server.Shared.Utils;

namespace Metin2Server.Shared.Encryption;

public class Handshake
{
    private const int MaxRandomValue = 1024 * 1024 - 1;

    public static uint CreateHandshake()
    {
        var crcBuffer = new byte[8];
        var value = Random.Shared.Next(0, MaxRandomValue);

        Array.Copy(BitConverter.GetBytes(value), 0, crcBuffer, 0, 4);
        Array.Copy(BitConverter.GetBytes(DateTimeUtils.GetUnixTime()), 0, crcBuffer, 4, 4);

        return Crc32.GetCrc32(crcBuffer);
    }
}