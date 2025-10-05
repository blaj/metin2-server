namespace Metin2Server.Shared.Protocol;

public enum ClientGameHeader : byte
{
    ClientVersion2 = 0xf1,
    Handshake = 0xff,
    CharacterCreate = 4,
    CharacterDelete = 5,
    CharacterSelect = 6,
    Move = 7,
    Entergame = 10,
    Empire = 90,
    MarkLogin = 100,
    Login2 = 109,
    Login3 = 111,
    StateChecker = 206,
}