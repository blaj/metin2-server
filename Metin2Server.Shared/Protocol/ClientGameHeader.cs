namespace Metin2Server.Network;

public enum ClientGameHeader : byte
{
    Unknown,
    Login,
    StateChecker = 206,
}