namespace Metin2Server.Channel.Features.Move;

public record ClientGameMovePacket(byte Func, byte Arg, byte Rot, int X, int Y, uint Time)
{
    public static int Size() =>
        sizeof(byte) +
        sizeof(byte) +
        sizeof(byte) +
        sizeof(int) +
        sizeof(int) +
        sizeof(int);
}