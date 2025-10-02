using Blaj.ReMetin2Server.Shared.Application.Utils;

namespace Blaj.ReMetin2Server.Common.Application.Encryptions;

public class Handshake
{
    private const int MaxRandomValue = 1024 * 1024 - 1;

    public static uint CreateHandshake(Func<uint, bool> predicate)
    {
        var crcBuffer = new byte[8];
        uint crc;

        do
        {
            var value = Random.Shared.Next(0, MaxRandomValue);

            Array.Copy(BitConverter.GetBytes(value), 0, crcBuffer, 0, 4);
            Array.Copy(BitConverter.GetBytes(DateTimeUtils.GetUnixTime()), 0, crcBuffer, 4, 4);

            crc = Crc32.GetCrc32(crcBuffer);
        } while (predicate.Invoke(crc) || crc == 0);

        return crc;
    }

    public static async Task<uint> CreateHandshake(Func<uint, Task<bool>> predicate)
    {
        var crcBuffer = new byte[8];
        uint crc;

        do
        {
            var value = Random.Shared.Next(0, MaxRandomValue);

            Array.Copy(BitConverter.GetBytes(value), 0, crcBuffer, 0, 4);
            Array.Copy(BitConverter.GetBytes(DateTimeUtils.GetUnixTime()), 0, crcBuffer, 4, 4);

            crc = Crc32.GetCrc32(crcBuffer);
        } while (await predicate.Invoke(crc) || crc == 0);

        return crc;
    }
}
