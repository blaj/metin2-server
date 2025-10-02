namespace Metin2Server.Shared.Protocol;

[Flags]
public enum SessionPhase
{
    None = 0,
    Handshake = 1 << 0, // np. wersja/seed, ping
    Login = 1 << 1, // login, captcha, 2FA
    SelectCharacter = 1 << 2, // lista postaci, wybór
    Loading = 1 << 3, // przejście na mapę
    InGame = 1 << 4, // gra właściwa
    Closing = 1 << 5,
    Auth = 1 << 6,
}