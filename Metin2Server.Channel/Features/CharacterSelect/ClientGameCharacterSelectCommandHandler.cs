using MediatR;
using Metin2Server.Channel.Features.Common.CharacterPoints;
using Metin2Server.Channel.Features.Common.MainCharacter;
using Metin2Server.Channel.Features.Common.SkillLevel;
using Metin2Server.Channel.Services;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Application.Phase;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Dto;
using Metin2Server.Shared.Enums;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Channel.Features.CharacterSelect;

public class ClientGameCharacterSelectCommandHandler : IRequestHandler<ClientGameCharacterSelectCommand>
{
    private readonly ISessionAccessor _sessionAccessor;
    private readonly CharacterService.CharacterServiceClient _characterServiceClient;
    private readonly AccountService.AccountServiceClient _accountServiceClient;
    private readonly GameCharacterService _gameCharacterService;

    public ClientGameCharacterSelectCommandHandler(
        ISessionAccessor sessionAccessor,
        CharacterService.CharacterServiceClient characterServiceClient,
        AccountService.AccountServiceClient accountServiceClient,
        GameCharacterService gameCharacterService)
    {
        _sessionAccessor = sessionAccessor;
        _characterServiceClient = characterServiceClient;
        _accountServiceClient = accountServiceClient;
        _gameCharacterService = gameCharacterService;
    }

    public async Task<Unit> Handle(ClientGameCharacterSelectCommand command, CancellationToken cancellationToken)
    {
        var currentSession = _sessionAccessor.Current;
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        if (currentSession.AccountId == null)
        {
            return GameClientPhasePacketUtils.AddToPacketCollector(
                currentSession,
                currentPacketOutCollector,
                SessionPhase.Closing);
        }

        var getAccountByIdGrpcResponse = await _accountServiceClient.GetAccountByIdAsync(
            new GetAccountByIdGrpcRequest { Id = currentSession.AccountId.Value },
            cancellationToken: cancellationToken);

        if (getAccountByIdGrpcResponse.ResultCase == GetAccountByIdGrpcResponse.ResultOneofCase.NotFound)
        {
            return GameClientPhasePacketUtils.AddToPacketCollector(
                currentSession,
                currentPacketOutCollector,
                SessionPhase.Closing);
        }

        var getCharacterByAccountIdAndIndexGrpcResponse =
            await _characterServiceClient.GetCharacterByAccountIdAndIndexAsync(
                new GetCharacterByAccountIdAndIndexGrpcRequest
                    { AccountId = currentSession.AccountId.Value, Index = command.Index },
                cancellationToken: cancellationToken);

        if (getCharacterByAccountIdAndIndexGrpcResponse.ResultCase ==
            GetCharacterByAccountIdAndIndexGrpcResponse.ResultOneofCase.NotFound)
        {
            return GameClientPhasePacketUtils.AddToPacketCollector(
                currentSession,
                currentPacketOutCollector,
                SessionPhase.Closing);
        }

        if (currentSession.GameCharacter != null || currentSession.Phase == SessionPhase.InGame)
        {
            return GameClientPhasePacketUtils.AddToPacketCollector(
                currentSession,
                currentPacketOutCollector,
                SessionPhase.Closing);
        }

        currentSession.Phase = SessionPhase.Loading;
        currentPacketOutCollector.Add(new GameClientPhasePacket(PhaseWireMapper.Map(currentSession.Phase)));

        var character = getCharacterByAccountIdAndIndexGrpcResponse.Character;
        var gameCharacter =
            _gameCharacterService.CreateGameCharacter(character, getAccountByIdGrpcResponse.Account.Empire, currentSession);

        if (gameCharacter == null)
        {
            return GameClientPhasePacketUtils.AddToPacketCollector(
                currentSession,
                currentPacketOutCollector,
                SessionPhase.Closing);
        }
        
        currentPacketOutCollector.Add(
            new GameClientMainCharacterPacket(
                gameCharacter.Vid,
                gameCharacter.PersitencePoints.Race,
                StringUtils.ToCBuffer(gameCharacter.Name, Constants.CharacterNameMaxLength + 1),
                gameCharacter.X,
                gameCharacter.Y,
                gameCharacter.Z,
                gameCharacter.Empire,
                gameCharacter.SkillGroup));

        var pointsForPacket = BuildPointsForPacket(gameCharacter);
        
        currentPacketOutCollector.Add(
            new GameClientCharacterPointsPacket(pointsForPacket));

        return Unit.Value;
    }

    private static int[] BuildPointsForPacket(GameCharacter gameCharacter)
    {
        var points = new int[Constants.PointMaxNum];

        points[(int)CharacterPointType.Level] = gameCharacter.PersitencePoints.Level;
        points[(int)CharacterPointType.Exp] = (int)gameCharacter.PersitencePoints.Exp;
        points[(int)CharacterPointType.NextExp] = (int)Constants.ExpTableList[gameCharacter.PersitencePoints.Level];
        points[(int)CharacterPointType.Hp] = gameCharacter.PersitencePoints.Hp;
        points[(int)CharacterPointType.MaxHp] = gameCharacter.RuntimePoints.MaxHp;
        points[(int)CharacterPointType.Sp] = gameCharacter.PersitencePoints.Sp;
        points[(int)CharacterPointType.MaxSp] = gameCharacter.RuntimePoints.MaxSp;
        points[(int)CharacterPointType.Gold] = (int)gameCharacter.PersitencePoints.Gold;
        points[(int)CharacterPointType.Stamina] = gameCharacter.PersitencePoints.Stamina;
        points[(int)CharacterPointType.MaxStamina] = gameCharacter.RuntimePoints.MaxStamina;

        foreach (var (key, value) in gameCharacter.RuntimePoints.Points)
        {
            points[(int)key] = value;
        }

        return points;
    }
}