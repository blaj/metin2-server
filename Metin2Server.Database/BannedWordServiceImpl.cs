using Grpc.Core;
using Metin2Server.Domain.Repositories;
using Metin2Server.Shared.DbContracts;

namespace Metin2Server.Database;

public class BannedWordServiceImpl : BannedWordService.BannedWordServiceBase
{
    private readonly IBannedWordRepository _bannedWordRepository;

    public BannedWordServiceImpl(IBannedWordRepository bannedWordRepository)
    {
        _bannedWordRepository = bannedWordRepository;
    }

    public override async Task<ExistsByWordResponse> ExistsByWord(
        ExistsByWordRequest request,
        ServerCallContext context)
    {
        return new ExistsByWordResponse
            { Exists = await _bannedWordRepository.ExistsByWordAsync(request.Word, context.CancellationToken) };
    }
}