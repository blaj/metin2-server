using Grpc.Core;
using Metin2Server.Database.Domain.Repositories;
using Metin2Server.Shared.DbContracts;

namespace Metin2Server.Database.Grpc;

public class BannedWordServiceImpl : BannedWordService.BannedWordServiceBase
{
    private readonly IBannedWordRepository _bannedWordRepository;

    public BannedWordServiceImpl(IBannedWordRepository bannedWordRepository)
    {
        _bannedWordRepository = bannedWordRepository;
    }

    public override async Task<ExistsByWordGrpcResponse> ExistsByWord(
        ExistsByWordGrpcRequest request,
        ServerCallContext context)
    {
        return new ExistsByWordGrpcResponse
            { Exists = await _bannedWordRepository.ExistsByWordAsync(request.Word, context.CancellationToken) };
    }
}