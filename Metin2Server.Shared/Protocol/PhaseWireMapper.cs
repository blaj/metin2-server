namespace Metin2Server.Shared.Protocol;

public static class PhaseWireMapper
{
    public static PhaseWire Map(SessionPhase sessionPhase) => sessionPhase switch
    {
        SessionPhase.Handshake => PhaseWire.Handshake,
        SessionPhase.Login => PhaseWire.Login,
        SessionPhase.SelectCharacter => PhaseWire.Select,
        SessionPhase.Loading => PhaseWire.Loading,
        SessionPhase.InGame => PhaseWire.Game,
        SessionPhase.Closing => PhaseWire.Close,
        SessionPhase.Auth => PhaseWire.Auth,
        _ => PhaseWire.Close,
    };
}