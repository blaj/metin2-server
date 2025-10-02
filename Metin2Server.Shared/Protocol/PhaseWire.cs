namespace Metin2Server.Shared.Protocol;

public enum PhaseWire : byte
{
    Close = 0,
    Handshake = 1,
    Login = 2,
    Select = 3,
    Loading = 4,
    Game = 5,
    Dead = 6,
    DbConnecting = 7,
    DbClient = 8,
    P2P = 9,
    Auth = 10,
}