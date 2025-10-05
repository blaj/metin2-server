namespace Metin2Server.Shared.Protocol;

public enum GameClientHeader : byte
{
    TimeSync = 0xfc,
    Phase = 0xfd,
    Handshake = 0xff,
    CharacterAdd = 1,
    LoginFailure = 7,
    CharacterCreateSuccess = 8,
    CharacterCreateFailure = 9,
    CharacterDeleteSuccess = 10,
    CharacterDeleteFailure = 11,
    CharacterPoints = 16,
    ItemSet = 21,
    LoginSuccessNewslot = 32,
    SkillLevel = 76,
    Empire = 90,
    Time = 106,
    MainCharacter = 113,
    Channel = 121,
    CharAdditionalInfo = 136,
    AuthSuccess = 150,
    RespondChannelStatus = 210
}