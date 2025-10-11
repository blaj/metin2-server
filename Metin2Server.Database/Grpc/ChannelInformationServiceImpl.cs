using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Metin2Server.Database.Domain.Repositories;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Utils;
using ChannelInformation = Metin2Server.Database.Domain.Entities.ChannelInformation;

namespace Metin2Server.Database.Grpc;

public class ChannelInformationServiceImpl : ChannelInformationService.ChannelInformationServiceBase
{
    private readonly IChannelInformationRepository _channelInformationRepository;

    public ChannelInformationServiceImpl(IChannelInformationRepository channelInformationRepository)
    {
        _channelInformationRepository = channelInformationRepository;
    }

    public override async Task<Empty> SendChannelInformation(
        SendChannelInformationGrpcRequest request,
        ServerCallContext context)
    {
        await _channelInformationRepository.UpsertAsync(new ChannelInformation
            {
                Port = GrpcUtils.ToUShortChecked(request.ChannelInformation.Port),
                ServerIndex = GrpcUtils.ToByteChecked(request.ChannelInformation.ServerIndex),
                Status = request.ChannelInformation.Status.ToEntity(),
                OnlinePlayers = request.ChannelInformation.OnlinePlayers,
                CpuLoad = request.ChannelInformation.CpuLoad,
                MemoryLoad = request.ChannelInformation.MemoryLoad,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            context.CancellationToken);

        return new Empty();
    }

    public override async Task<GetChannelInformationsGrpcResponse> GetChannelInformations(
        Empty request,
        ServerCallContext context)
    {
        var channelInformationList = await _channelInformationRepository.GetAllAsync(context.CancellationToken);

        return new GetChannelInformationsGrpcResponse
        {
            Entries =
            {
                channelInformationList.Select(channelInformation => new ChannelInformationGrpc
                {
                    Port = channelInformation.Port,
                    ServerIndex = channelInformation.ServerIndex,
                    Status = channelInformation.Status.ToProto(),
                    OnlinePlayers = channelInformation.OnlinePlayers,
                    CpuLoad = channelInformation.CpuLoad,
                    MemoryLoad = channelInformation.MemoryLoad
                })
            }
        };
    }

    public override async Task<Empty> RemoveChannelInformation(
        RemoveChannelInformationGrpcRequest request,
        ServerCallContext context)
    {
        await _channelInformationRepository.RemoveAsync(
            GrpcUtils.ToUShortChecked(request.Port),
            context.CancellationToken);

        return new Empty();
    }
}