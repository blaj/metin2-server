using System.Security.Cryptography;
using System.Text.Json;
using Metin2Server.Database.Domain.Entities;
using Metin2Server.Database.Domain.Repositories;
using StackExchange.Redis;

namespace Metin2Server.Database.Repositories;

public class LoginKeyRepository : ILoginKeyRepository
{
    private const string ConsumeLua = @"
        local v = redis.call('GET', KEYS[1])
        if not v then return {0} end
        local obj = cjson.decode(v)
        if obj.consumed == true then return {0} end
        if ARGV[1] ~= '0' and tostring(obj.account_id) ~= ARGV[1] then return {0} end
        if obj.expected_ip and ARGV[2] ~= '' and obj.expected_ip ~= ARGV[2] then return {0} end
        obj.consumed = true
        redis.call('SET', KEYS[1], cjson.encode(obj), 'KEEPTTL')
        return {1, obj.account_id, obj.expire_at}";

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public LoginKeyRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<LoginKey> IssueAsync(
        long accountId,
        long? sessionId,
        TimeSpan ttl,
        string? expectedIp,
        CancellationToken cancellationToken)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var effectiveTtl = ttl <= TimeSpan.Zero ? TimeSpan.FromSeconds(60) : ttl;

        uint key;
        string redisKey;
        DateTimeOffset expireAt;

        do
        {
            key = (uint)RandomNumberGenerator.GetInt32(1, int.MaxValue);
            redisKey = RedisKey(key);
            expireAt = DateTimeOffset.UtcNow.Add(effectiveTtl);

            var json = JsonSerializer.Serialize(new
            {
                key,
                account_id = accountId,
                expire_at = expireAt.ToUnixTimeSeconds(),
                consumed = false,
                issued_session_id = sessionId,
                expected_ip = string.IsNullOrWhiteSpace(expectedIp) ? null : expectedIp
            });

            var ok = await db.StringSetAsync(redisKey, json, effectiveTtl, When.NotExists);

            if (ok)
            {
                break;
            }
        } while (true);

        return new LoginKey
        {
            Key = key,
            AccountId = accountId,
            ExpireAt = expireAt,
            Consumed = false,
            IssuedSessionId = sessionId,
            ExpectedIp = string.IsNullOrWhiteSpace(expectedIp) ? null : expectedIp
        };
    }

    public async Task<(bool ok, LoginKey? entry)> TryConsumeAsync(
        uint key,
        long? expectedAccountId,
        string? clientIp,
        CancellationToken cancellationToken)
    {
        var db = _connectionMultiplexer.GetDatabase();

        var res = (RedisResult[]?)await db.ScriptEvaluateAsync(
            ConsumeLua,
            [RedisKey(key)],
            [
                expectedAccountId.HasValue ? expectedAccountId.Value.ToString() : "0",
                clientIp ?? string.Empty
            ]);

        if (res == null || res.Length == 0 || (int)res[0] == 0)
        {
            return (false, null);
        }

        var accountId = (long)res[1];
        var expireUnix = (long)res[2];

        var entry = new LoginKey
        {
            Key = key,
            AccountId = accountId,
            ExpireAt = DateTimeOffset.FromUnixTimeSeconds(expireUnix),
            Consumed = true
        };

        return (true, entry);
    }

    private static string RedisKey(uint key) => $"loginkey:{key}";
}