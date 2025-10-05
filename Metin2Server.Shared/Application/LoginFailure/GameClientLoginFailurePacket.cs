using Metin2Server.Shared.Common;

namespace Metin2Server.Shared.Application.LoginFailure;

public record GameClientLoginFailurePacket(string Status)
{
    public static int Size() => sizeof(byte) * (Constants.LoginStatusMaxLength + 1);
}