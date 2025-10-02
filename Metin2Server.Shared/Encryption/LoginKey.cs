namespace Metin2Server.Shared.Encryption;

public record LoginKey(uint Key, uint ExpireTime, uint? PanamaKey);