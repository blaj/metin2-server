using MediatR;
using Metin2Server.Shared.Enums;

namespace Metin2Server.Channel.Features.CharacterCreate;

public record ClientGameCharacterCreateCommand(byte Index, string Name, Race Race, byte Shape) : IRequest;