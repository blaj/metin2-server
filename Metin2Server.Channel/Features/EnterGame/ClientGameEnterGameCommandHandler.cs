using MediatR;
using Metin2Server.Channel.Features.Common.Channel;
using Metin2Server.Channel.Features.Common.CharacterAdd;
using Metin2Server.Channel.Features.Common.CharacterAdditional;
using Metin2Server.Channel.Features.Common.SkillLevel;
using Metin2Server.Channel.Features.Common.Time;
using Metin2Server.Shared.Application;
using Metin2Server.Shared.Application.Phase;
using Metin2Server.Shared.Common;
using Metin2Server.Shared.Enums;
using Metin2Server.Shared.Protocol;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Channel.Features.EnterGame;

public class ClientGameEnterGameCommandHandler : IRequestHandler<ClientGameEnterGameCommand>
{
    private readonly ISessionAccessor _sessionAccessor;

    public ClientGameEnterGameCommandHandler(ISessionAccessor sessionAccessor)
    {
        _sessionAccessor = sessionAccessor;
    }

    public async Task<Unit> Handle(ClientGameEnterGameCommand request, CancellationToken cancellationToken)
    {
        var currentSession = _sessionAccessor.Current;
        var currentPacketOutCollector = _sessionAccessor.CurrentPacketOutCollector;

        if (currentSession.GameCharacter == null)
        {
            return GameClientPhasePacketUtils.AddToPacketCollector(
                currentSession,
                currentPacketOutCollector,
                SessionPhase.Closing);
        }

        GameClientPhasePacketUtils.AddToPacketCollector(
            currentSession,
            currentPacketOutCollector,
            SessionPhase.InGame);
        
        var gameCharacter = currentSession.GameCharacter;

        currentPacketOutCollector.Add(new GameClientCharacterAddPacket(
            gameCharacter.Vid,
            0,
            gameCharacter.X,
            gameCharacter.Y,
            gameCharacter.Z,
            CharacterType.Pc,
            gameCharacter.PersitencePoints.Race,
            100,
            100,
            0,
            [0, 0]));
        
        currentPacketOutCollector.Add(new GameClientCharacterAdditionalPacket(
            gameCharacter.Vid,
            StringUtils.ToCBuffer(gameCharacter.Name, Constants.CharacterNameMaxLength + 1),
            [0, 0, 1001, 1001],
            gameCharacter.Empire,
            0,
            gameCharacter.PersitencePoints.Level,
            0,
            0,
            0));
        
        currentPacketOutCollector.Add(new GameClientSkillLevelPacket(new int[255]));
        
        currentPacketOutCollector.Add(new GameClientTimePacket(DateTimeUtils.GetUnixTime()));
        
        currentPacketOutCollector.Add(new GameClientChannelPacket(1));

        return Unit.Value;
    }
}