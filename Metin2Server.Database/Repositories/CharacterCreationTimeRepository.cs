using Metin2Server.Database.Domain.Repositories;
using StackExchange.Redis;

namespace Metin2Server.Database.Repositories;

public class CharacterCreationTimeRepository : ICharacterCreationTimeRepository
{
    private const string ConsumeLua = @"
        local last = redis.call('GET', KEYS[1])
        local now = tonumber(ARGV[1])
        local window = tonumber(ARGV[2])
        if last and (now - tonumber(last)) < window then
          return 0
        else
          redis.call('SET', KEYS[1], now, 'EX', window)
          return 1
        end";

    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly TimeProvider _timeProvider;

    public CharacterCreationTimeRepository(
        IConnectionMultiplexer connectionMultiplexer,
        TimeProvider timeProvider)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _timeProvider = timeProvider;
    }

    public async Task<bool> TryConsumeAsync(long accountId, TimeSpan window, CancellationToken cancellationToken)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var nowSeconds = _timeProvider.GetUtcNow().ToUnixTimeSeconds();

        var res = (int)(long)await db.ScriptEvaluateAsync(
            ConsumeLua,
            [RedisKey(accountId)],
            [nowSeconds, (long)window.TotalSeconds]);

        return res == 1;
    }

    private static string RedisKey(long accountId) => $"account:{accountId}:character_create";
}