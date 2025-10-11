using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Extensions;

public static class ChannelStatusExtensions
{
    public static ChannelStatusGrpc ToProto(this ChannelStatus channelStatus) => channelStatus switch
    {
        ChannelStatus.Offline => ChannelStatusGrpc.Offline,
        ChannelStatus.Online => ChannelStatusGrpc.Online,
        ChannelStatus.Busy => ChannelStatusGrpc.Busy,
        ChannelStatus.Full => ChannelStatusGrpc.Full,
        _ => ChannelStatusGrpc.Offline
    };

    public static ChannelStatus ToEntity(this ChannelStatusGrpc channelStatus) => channelStatus switch
    {
        ChannelStatusGrpc.Offline => ChannelStatus.Offline,
        ChannelStatusGrpc.Online => ChannelStatus.Online,
        ChannelStatusGrpc.Busy => ChannelStatus.Busy,
        ChannelStatusGrpc.Full => ChannelStatus.Full,
        _ => ChannelStatus.Offline
    };
}