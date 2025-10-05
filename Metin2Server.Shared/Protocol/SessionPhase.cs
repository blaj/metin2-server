namespace Metin2Server.Shared.Protocol;

[Flags]
public enum SessionPhase
{
    None = 0,
    Handshake = 1 << 0,
    Login = 1 << 1, 
    SelectCharacter = 1 << 2, 
    Loading = 1 << 3, 
    InGame = 1 << 4,
    Closing = 1 << 5,
    Auth = 1 << 6,
}