using MediatR;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Application.AuthSuccess;
using Metin2Server.Shared.Application.LoginFailure;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Encryption;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Auth.Features.Login3;

public class ClientGameLogin3CommandHandler : IRequestHandler<ClientGameLogin3Command>
{
    private readonly DbService.DbServiceClient _dbServiceClient;
    private readonly LoginKeyService.LoginKeyServiceClient _loginKeyServiceClient;
    private readonly PasswordHasherService _passwordHasherService;
    private readonly ISessionAccessor _sessionAccessor;

    public ClientGameLogin3CommandHandler(
        DbService.DbServiceClient dbServiceClient,
        LoginKeyService.LoginKeyServiceClient loginKeyServiceClient,
        PasswordHasherService passwordHasherService,
        ISessionAccessor sessionAccessor)
    {
        _dbServiceClient = dbServiceClient;
        _loginKeyServiceClient = loginKeyServiceClient;
        _passwordHasherService = passwordHasherService;
        _sessionAccessor = sessionAccessor;
    }

    public async Task<Unit> Handle(ClientGameLogin3Command command, CancellationToken cancellationToken)
    {
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;
        
        if (!IsValidLoginString(command.Username))
        {
            currentPacketOutCollector.Add(new GameClientLoginFailurePacket("NOID"));

            return Unit.Value;
        }

        var getAccountByLoginResponse =
            await _dbServiceClient.GetAccountByLoginAsync(
                new GetAccountByLoginRequest() { Login = command.Username },
                cancellationToken: cancellationToken);

        if (getAccountByLoginResponse.ResultCase == GetAccountByLoginResponse.ResultOneofCase.NotFound)
        {
            currentPacketOutCollector.Add(new GameClientLoginFailurePacket("NOID"));

            return Unit.Value;
        }

        if (!getAccountByLoginResponse.Account.IsActive)
        {
            currentPacketOutCollector.Add(new GameClientLoginFailurePacket("NOTAVAIL"));

            return Unit.Value;
        }

        if (!_passwordHasherService.Verify(command.Password, getAccountByLoginResponse.Account.Password))
        {
            currentPacketOutCollector.Add(new GameClientLoginFailurePacket("WRONGPWD"));

            return Unit.Value;
        }

        var currentSession = _sessionAccessor.Current;

        var issueLoginKeyResponse = await _loginKeyServiceClient.IssueAsync(
            new IssueLoginKeyRequest
            {
                AccountId = getAccountByLoginResponse.Account.Id,
                IssuedSessionId = currentSession.Id,
                TtlSeconds = 60
            },
            cancellationToken: cancellationToken);

        var loginKey = issueLoginKeyResponse.Key;
        currentSession.LoginKey = loginKey;

        currentPacketOutCollector.Add(new GameClientAuthSuccessPacket(loginKey, 1));

        return Unit.Value;
    }

    private LoginKey CreateLoginKey(uint[] adwClientKey)
    {
        var key = (uint)Random.Shared.Next(1, int.MaxValue);
        var panamaKey = key ^
                        adwClientKey[0] ^
                        adwClientKey[1] ^
                        adwClientKey[2] ^
                        adwClientKey[3];

        return new LoginKey(key, DateTimeUtils.GetUnixTime(), panamaKey);
    }

    private static bool IsValidLoginString(string login) => !string.IsNullOrEmpty(login) && login.Length >= 2;
}