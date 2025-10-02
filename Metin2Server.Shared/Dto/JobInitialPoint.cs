namespace Metin2Server.Shared.Dto;

public record JobInitialPoint(
    ushort St,
    ushort Ht,
    ushort Dx,
    ushort Iq,
    uint MaxHp,
    uint MaxSp,
    ushort HpPerHt,
    ushort SpPerIq,
    ushort HpPerLevelBegin,
    ushort HpPerLevelEnd,
    ushort SpPerLevelBegin,
    ushort SpPerLevelEnd,
    ushort MaxStamina,
    ushort StaminaPerCon,
    ushort StaminaPerLevelBegin,
    ushort StaminaPerLevelEnd);