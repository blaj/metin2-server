using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Metin2Server.Database.Domain.Repositories;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Database.Grpc;

public class AccountServiceImpl : AccountService.AccountServiceBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AccountServiceImpl(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<GetAccountByLoginGrpcResponse> GetAccountByLogin(
        GetAccountByLoginGrpcRequest request,
        ServerCallContext context)
    {
        var account = await _accountRepository.FindOneByLoginAsync(request.Login, context.CancellationToken);

        if (account == null)
        {
            return new GetAccountByLoginGrpcResponse { NotFound = new Empty() };
        }

        return new GetAccountByLoginGrpcResponse
        {
            Account = new AccountGrpc
            {
                Id = account.Id,
                Username = account.Login,
                Password = account.Password,
                IsActive = account.Active,
                Empire = account.Empire.HasValue
                    ? (EmpireGrpc)account.Empire.Value
                    : EmpireGrpc.Unknown
            }
        };
    }

    public override async Task<GetAccountByIdGrpcResponse> GetAccountById(
        GetAccountByIdGrpcRequest request,
        ServerCallContext context)
    {
        var account = await _accountRepository.FindOneByIdAsync(request.Id, context.CancellationToken);

        if (account == null)
        {
            return new GetAccountByIdGrpcResponse { NotFound = new Empty() };
        }

        return new GetAccountByIdGrpcResponse
        {
            Account = new AccountGrpc
            {
                Id = account.Id,
                Username = account.Login,
                Password = account.Password,
                IsActive = account.Active,
                Empire = account.Empire.HasValue
                    ? (EmpireGrpc)account.Empire.Value
                    : EmpireGrpc.Unknown
            }
        };
    }

    public override async Task<ChangeAccountEmpireGrpcResponse> ChangeAccountEmpire(
        ChangeAccountEmpireGrpcRequest request,
        ServerCallContext context)
    {
        var account = await _accountRepository.FindOneByIdAsync(request.Id, context.CancellationToken);

        if (account == null)
        {
            return new ChangeAccountEmpireGrpcResponse
            {
                Ok = false
            };
        }

        if (request.Empire == EmpireGrpc.Unknown)
        {
            return new ChangeAccountEmpireGrpcResponse
            {
                Ok = false
            };
        }

        account.Empire = (Empire)request.Empire;

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return new ChangeAccountEmpireGrpcResponse
        {
            Ok = true
        };
    }

    public override async Task<GetPrivateCodeByIdGrpcResponse> GetPrivateCodeById(
        GetPrivateCodeByIdGrpcRequest request,
        ServerCallContext context)
    {
        var account = await _accountRepository.FindOneByIdAsync(request.Id, context.CancellationToken);

        if (account == null)
        {
            return new GetPrivateCodeByIdGrpcResponse
            {
                NotFound = new Empty()
            };
        }
        
        return new GetPrivateCodeByIdGrpcResponse
        {
            PrivateCode = account.PrivateCode
        };
    }
}