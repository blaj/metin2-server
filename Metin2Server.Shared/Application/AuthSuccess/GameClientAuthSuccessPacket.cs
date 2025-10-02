namespace Metin2Server.Shared.Application.AuthSuccess;

public record GameClientAuthSuccessPacket(uint LoginKey, byte Result);