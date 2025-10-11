using Grpc.Core;
using Metin2Server.Database.Domain.Repositories;
using Metin2Server.Shared.DbContracts;

namespace Metin2Server.Database.Grpc;

public class LoginKeyServiceImpl : LoginKeyService.LoginKeyServiceBase
{
    private readonly ILoginKeyRepository _loginKeyRepository;

    public LoginKeyServiceImpl(ILoginKeyRepository loginKeyRepository)
    {
        _loginKeyRepository = loginKeyRepository;
    }

    public override async Task<IssueLoginKeyGrpcResponse> Issue(IssueLoginKeyGrpcRequest request, ServerCallContext context)
    {
        var ttl = request.TtlSeconds > 0 ? TimeSpan.FromSeconds(request.TtlSeconds) : TimeSpan.FromSeconds(60);

        var entry = await _loginKeyRepository.IssueAsync(
            request.AccountId,
            request.IssuedSessionId == 0 ? null : request.IssuedSessionId,
            ttl,
            string.IsNullOrWhiteSpace(request.ExpectedIp) ? null : request.ExpectedIp,
            context.CancellationToken);

        return new IssueLoginKeyGrpcResponse
        {
            Key = entry.Key,
            ExpireAtUnix = entry.ExpireAt.ToUnixTimeSeconds()
        };
    }

    public override async Task<ConsumeLoginKeyGrpcResponse> Consume(ConsumeLoginKeyGrpcRequest request, ServerCallContext context)
    {
        var (ok, entry) = await _loginKeyRepository.TryConsumeAsync(
            request.Key,
            request.ExpectedAccountId > 0 ? request.ExpectedAccountId : null,
            string.IsNullOrWhiteSpace(request.ClientIp) ? null : request.ClientIp,
            context.CancellationToken);

        if (!ok || entry is null)
        {
            return new ConsumeLoginKeyGrpcResponse { Ok = false };
        }

        return new ConsumeLoginKeyGrpcResponse
        {
            Ok = true,
            AccountId = entry.AccountId,
            ExpireAtUnix = entry.ExpireAt.ToUnixTimeSeconds()
        };
    }
}