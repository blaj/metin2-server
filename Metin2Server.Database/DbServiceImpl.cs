using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Metin2Server.Domain.Repositories;
using Metin2Server.Shared.DbContracts;

namespace Metin2Server.Database;

public class DbServiceImpl : DbService.DbServiceBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DbServiceImpl(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<GetAccountByLoginResponse> GetAccountByLogin(
        GetAccountByLoginRequest request,
        ServerCallContext context)
    {
        var account = await _accountRepository.FindOneByLoginAsync(request.Login, context.CancellationToken);

        if (account == null)
        {
            return new GetAccountByLoginResponse { NotFound = new Empty() };
        }

        return new GetAccountByLoginResponse
        {
            Account = new Account
            {
                Id = account.Id,
                Username = account.Login,
                Password = account.Password,
                IsActive = account.Active,
                Empire = account.Empire.HasValue
                    ? (Shared.DbContracts.Common.Empire)account.Empire.Value
                    : Shared.DbContracts.Common.Empire.Unknown
            }
        };
    }

    public override async Task<GetAccountByIdResponse> GetAccountById(
        GetAccountByIdRequest request,
        ServerCallContext context)
    {
        var account = await _accountRepository.FindOneByIdAsync(request.Id, context.CancellationToken);

        if (account == null)
        {
            return new GetAccountByIdResponse { NotFound = new Empty() };
        }

        return new GetAccountByIdResponse
        {
            Account = new Account
            {
                Id = account.Id,
                Username = account.Login,
                Password = account.Password,
                IsActive = account.Active,
                Empire = account.Empire.HasValue
                    ? (Shared.DbContracts.Common.Empire)account.Empire.Value
                    : Shared.DbContracts.Common.Empire.Unknown
            }
        };
    }

    public override async Task<ChangeAccountEmpireResponse> ChangeAccountEmpire(
        ChangeAccountEmpireRequest request,
        ServerCallContext context)
    {
        var account = await _accountRepository.FindOneByIdAsync(request.Id, context.CancellationToken);

        if (account == null)
        {
            return new ChangeAccountEmpireResponse
            {
                Ok = false
            };
        }

        if (request.Empire == Shared.DbContracts.Common.Empire.Unknown)
        {
            return new ChangeAccountEmpireResponse
            {
                Ok = false
            };
        }

        account.Empire = (Shared.Enums.Empire)request.Empire;

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return new ChangeAccountEmpireResponse
        {
            Ok = true
        };
    }

    public override async Task<GetPrivateCodeByIdResponse> GetPrivateCodeById(
        GetPrivateCodeByIdRequest request,
        ServerCallContext context)
    {
        var account = await _accountRepository.FindOneByIdAsync(request.Id, context.CancellationToken);

        if (account == null)
        {
            return new GetPrivateCodeByIdResponse
            {
                NotFound = new Empty()
            };
        }
        
        return new GetPrivateCodeByIdResponse
        {
            PrivateCode = account.PrivateCode
        };
    }
}