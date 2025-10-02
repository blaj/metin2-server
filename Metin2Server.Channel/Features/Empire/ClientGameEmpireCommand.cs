using MediatR;

namespace Metin2Server.Channel.Features.Empire;

public record ClientGameEmpireCommand(Shared.Enums.Empire Empire) : IRequest;