using Metin2Server.Domain.Entities;
using Metin2Server.Domain.Repositories;
using Metin2Server.Shared.Enums;
using StackExchange.Redis;

namespace Metin2Server.Database.Repositories;

public class ChannelInformationRepository : IChannelInformationRepository
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(30);
    private static readonly RedisKey PortsKey = "channel_info:ports";

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public ChannelInformationRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task UpsertAsync(ChannelInformation channelInformation, CancellationToken cancellationToken)
    {
        var transaction = _connectionMultiplexer.GetDatabase().CreateTransaction();
        var key = RedisKey(channelInformation.Port);

        _ = transaction.HashSetAsync(
            key,
            [
                new("port", (int)channelInformation.Port),
                new("serverIndex", (short)channelInformation.ServerIndex),
                new("status", (short)channelInformation.Status),
                new("onlinePlayers", channelInformation.OnlinePlayers),
                new("cpuLoad", channelInformation.CpuLoad),
                new("memoryLoad", channelInformation.MemoryLoad),
                new("updatedAt", channelInformation.UpdatedAt.ToUnixTimeMilliseconds())
            ]);
        _ = transaction.KeyExpireAsync(key, Ttl);
        _ = transaction.SetAddAsync(PortsKey, (int)channelInformation.Port);

        await transaction.ExecuteAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelInformation>> GetAllAsync(CancellationToken cancellationToken)
    {
        var database = _connectionMultiplexer.GetDatabase();
        var ports = await database.SetMembersAsync(PortsKey).ConfigureAwait(false);
        var tasks = new Task<HashEntry[]>[ports.Length];
        var ttlTasks = new Task<TimeSpan?>[ports.Length];

        for (var i = 0; i < ports.Length; i++)
        {
            var port = (ushort)(int)ports[i];
            var key = RedisKey(port);

            tasks[i] = database.HashGetAllAsync(key);
            ttlTasks[i] = database.KeyTimeToLiveAsync(key);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        await Task.WhenAll(ttlTasks).ConfigureAwait(false);

        var list = new List<ChannelInformation>(ports.Length);

        for (var i = 0; i < ports.Length; i++)
        {
            var port = (ushort)(int)ports[i];
            var entries = tasks[i].Result;

            if (entries.Length == 0)
            {
                continue;
            }

            var map = entries.ToDictionary(e => (string)e.Name!, e => e.Value);
            var ttl = ttlTasks[i].Result;

            var channelInformation = new ChannelInformation
            {
                Port = port,
                ServerIndex = (byte)(int)map["serverIndex"],
                Status = ttl.HasValue && ttl.Value.TotalMilliseconds > 0
                    ? (ChannelStatus)(int)map["status"]
                    : ChannelStatus.Offline,
                OnlinePlayers = (uint)map["onlinePlayers"],
                CpuLoad = (double)map["cpuLoad"],
                MemoryLoad = (double)map["memoryLoad"],
                UpdatedAt = DateTimeOffset.FromUnixTimeMilliseconds((long)map["updatedAt"])
            };

            list.Add(channelInformation);
        }

        return list
            .OrderBy(channelInformation => channelInformation.ServerIndex)
            .ThenBy(channelInformation => channelInformation.Port);
    }

    public async Task RemoveAsync(ushort port, CancellationToken cancellationToken)
    {
        var key = RedisKey(port);
        var transaction = _connectionMultiplexer.GetDatabase().CreateTransaction();
        
        _ = transaction.KeyDeleteAsync(key);
        _ = transaction.SetRemoveAsync(PortsKey, (int)port);
        
        await transaction.ExecuteAsync().ConfigureAwait(false);
    }
    
    private static string RedisKey(ushort port) => $"channel_info:{port}";
}