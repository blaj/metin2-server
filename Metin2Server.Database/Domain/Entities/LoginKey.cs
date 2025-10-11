namespace Metin2Server.Database.Domain.Entities;

public class LoginKey
{
    public required uint Key { get; set; }
    public required long AccountId { get; set; }
    public required DateTimeOffset ExpireAt { get; set; }
    public required bool Consumed { get; set; }
    public long? IssuedSessionId { get; set; } = null;
    public string? ExpectedIp { get; set; } = null;
}