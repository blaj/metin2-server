namespace Metin2Server.Shared.Protocol;

public enum ClientGameHeader : byte
{
    Handshake = 0xff,
    CharacterCreate = 4,
    CharacterDelete = 5,
    CharacterSelect = 6,
    Empire = 90,
    Login2 = 109,
    Login3 = 111,
    StateChecker = 206,
}