using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Metin2Server.Database.Domain.Entities;
using Metin2Server.Database.Domain.Repositories;
using Metin2Server.Database.Extensions;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.DbContracts;
using Metin2Server.Shared.Dto;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Database.Grpc;

public class CharacterServiceImpl : CharacterService.CharacterServiceBase
{
    private readonly ICharacterCreationTimeRepository _characterCreationTimeRepository;
    private readonly ICharacterRepository _characterRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CharacterServiceImpl(
        ICharacterCreationTimeRepository characterCreationTimeRepository,
        ICharacterRepository characterRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _characterCreationTimeRepository = characterCreationTimeRepository;
        _characterRepository = characterRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<TryBeginCharacterCreationGrpcResponse> TryBeginCharacterCreation(
        TryBeginCharacterCreationGrpcRequest request,
        ServerCallContext context)
    {
        return new TryBeginCharacterCreationGrpcResponse
        {
            Allowed = await _characterCreationTimeRepository.TryConsumeAsync(
                request.AccountId,
                TimeSpan.FromSeconds(Constants.SecondIntervalBetweenCharacterCreate),
                context.CancellationToken)
        };
    }

    public override async Task<ExistsByIndexAndAccountIdGrpcResponse> ExistsByIndexAndAccountId(
        ExistsByIndexAndAccountIdGrpcRequest request,
        ServerCallContext context)
    {
        return new ExistsByIndexAndAccountIdGrpcResponse
        {
            Exists = await _characterRepository.ExistsByIndexAndAccountIdAsync(
                (byte)request.Index,
                request.AccountId,
                context.CancellationToken)
        };
    }

    public override async Task<ExistsByNameGrpcResponse> ExistsByName(
        ExistsByNameGrpcRequest request,
        ServerCallContext context)
    {
        return new ExistsByNameGrpcResponse
        {
            Exists = await _characterRepository.ExistsByName(request.Name, context.CancellationToken)
        };
    }

    public override async Task<CreateCharacterGrpcResponse> CreateCharacter(
        CreateCharacterGrpcRequest request,
        ServerCallContext context)
    {
        var account = await _accountRepository.FindOneByIdAsync(request.AccountId, context.CancellationToken);

        if (account == null || account.Empire == null)
        {
            return new CreateCharacterGrpcResponse { NotFound = new Empty() };
        }

        var race = request.Race.ToEntity();
        var startPositionCoords = GetStartPositionCoords((Shared.Enums.Empire)account.Empire);
        var jobInitialPoints = race.ToJob().GetInitialPoint();

        var character = new Character()
        {
            Name = request.Name,
            Race = race,
            Index = GrpcUtils.ToByteChecked(request.Index),
            Account = account,
            PartMain = GrpcUtils.ToUShortChecked(request.Shape),
            PartBase = GrpcUtils.ToByteChecked(request.Shape),
            Coordinates = new Character.CoordinatesInfo
            {
                Dir = 0,
                X = startPositionCoords.X,
                Y = startPositionCoords.Y,
                Z = 0,
            },
            Statistics = new Character.StatisticsInfo
            {
                Hp = (int)(jobInitialPoints.MaxHp + jobInitialPoints.Ht * jobInitialPoints.HpPerHt),
                Sp = (int)(jobInitialPoints.MaxSp + jobInitialPoints.Iq * jobInitialPoints.SpPerIq),
                Stamina = jobInitialPoints.MaxStamina,
                St = jobInitialPoints.St,
                Ht = jobInitialPoints.Ht,
                Dx = jobInitialPoints.Dx,
                Iq = jobInitialPoints.Iq,
            },
        };

        await _characterRepository.AddAsync(character, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return new CreateCharacterGrpcResponse { Character = character.ToProto() };
    }

    public override async Task<GetCharactersByAccountIdGrpcResponse> GetCharactersByAccountId(
        GetCharactersByAccountIdGrpcRequest request,
        ServerCallContext context)
    {
        var characters =
            await _characterRepository.FindAllByAccountIdOrderByIndexAsync(
                request.AccountId,
                context.CancellationToken);

        return new GetCharactersByAccountIdGrpcResponse
        {
            Characters = { characters.Select(character => character.ToProto()) }
        };
    }

    public override async Task<GetCharacterLevelByAccountIdAndIndexGrpcResponse> GetCharacterLevelByAccountIdAndIndex(
        GetCharacterLevelByAccountIdAndIndexGrpcRequest request,
        ServerCallContext context)
    {
        var character =
            await _characterRepository.FindOneByAccountIdAndIndex(
                request.AccountId,
                GrpcUtils.ToByteChecked(request.Index),
                context.CancellationToken);

        if (character == null)
        {
            return new GetCharacterLevelByAccountIdAndIndexGrpcResponse
            {
                NotFound = new Empty()
            };
        }

        return new GetCharacterLevelByAccountIdAndIndexGrpcResponse
        {
            Level = character.Statistics.Level
        };
    }

    public override async Task<DeleteCharacterByAccountIdAndIndexGrpcResponse> DeleteCharacterByAccountIdAndIndex(
        DeleteCharacterByAccountIdAndIndexGrpcRequest request,
        ServerCallContext context)
    {
        var character = await _characterRepository.FindOneByAccountIdAndIndex(
            request.AccountId,
            GrpcUtils.ToByteChecked(request.Index),
            context.CancellationToken);

        if (character == null)
        {
            return new DeleteCharacterByAccountIdAndIndexGrpcResponse
            {
                Success = false
            };
        }

        await _characterRepository.DeleteAsync(character.Id, context.CancellationToken);
        await _unitOfWork.SaveChangesAsync(context.CancellationToken);

        return new DeleteCharacterByAccountIdAndIndexGrpcResponse
        {
            Success = true
        };
    }

    public override async Task<GetCharacterByAccountIdAndIndexGrpcResponse> GetCharacterByAccountIdAndIndex(
        GetCharacterByAccountIdAndIndexGrpcRequest request, 
        ServerCallContext context)
    {
        var character = await _characterRepository.FindOneByAccountIdAndIndex(
            request.AccountId,
            GrpcUtils.ToByteChecked(request.Index),
            context.CancellationToken);

        if (character == null)
        {
            return new GetCharacterByAccountIdAndIndexGrpcResponse
            {
                NotFound = new Empty()
            };
        }

        return new GetCharacterByAccountIdAndIndexGrpcResponse
        {
            Character = character.ToProto()
        };
    }

    private static Coords GetStartPositionCoords(Shared.Enums.Empire empire)
    {
        var startPositionCoord = empire.GetStartPosition();

        return new Coords
        {
            X = startPositionCoord.X + Random.Shared.Next(-300, 300),
            Y = startPositionCoord.Y + Random.Shared.Next(-300, 300),
            Z = 0
        };
    }
}