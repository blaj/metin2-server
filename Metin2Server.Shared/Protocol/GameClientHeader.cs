namespace Metin2Server.Shared.Protocol;

public enum GameClientHeader : byte
{
    TimeSync = 0xfc,
    Phase = 0xfd,
    Handshake = 0xff,
    LoginFailure = 7,
    CharacterCreateSuccess = 8,
    CharacterCreateFailure = 9,
    CharacterDeleteSuccess = 10,
    CharacterDeleteFailure = 11,
    LoginSuccessNewslot = 32,
    Empire = 90,
    AuthSuccess = 150,
    RespondChannelStatus = 210
}