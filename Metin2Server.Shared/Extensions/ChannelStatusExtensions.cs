using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Extensions;

public static class ChannelStatusExtensions
{
    public static DbContracts.ChannelStatus ToProto(this ChannelStatus channelStatus) => channelStatus switch
    {
        ChannelStatus.Offline => DbContracts.ChannelStatus.Offline,
        ChannelStatus.Online => DbContracts.ChannelStatus.Online,
        ChannelStatus.Busy => DbContracts.ChannelStatus.Busy,
        ChannelStatus.Full => DbContracts.ChannelStatus.Full,
        _ => DbContracts.ChannelStatus.Offline
    };

    public static ChannelStatus ToEntity(this DbContracts.ChannelStatus channelStatus) => channelStatus switch
    {
        DbContracts.ChannelStatus.Offline => ChannelStatus.Offline,
        DbContracts.ChannelStatus.Online => ChannelStatus.Online,
        DbContracts.ChannelStatus.Busy => ChannelStatus.Busy,
        DbContracts.ChannelStatus.Full => ChannelStatus.Full,
        _ => ChannelStatus.Offline
    };
}