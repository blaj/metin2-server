using MediatR;

namespace Metin2Server.Channel.Features.CharacterDelete;

public record ClientGameCharacterDeleteCommand(byte Index, string PrivateCode) : IRequest;