using MediatR;

namespace Metin2Server.Channel.Features.CharacterSelect;

public record ClientGameCharacterSelectCommand(byte Index) : IRequest;