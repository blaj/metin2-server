namespace Metin2Server.Shared.Dto;

public class GameItem
{
    public required long Id { get; init; }
    public required uint Vnum { get; init; }
    public required ItemDefinition ItemDefinition { get; init; }

    public uint Count { get; set; } = 1;
    public int[] Sockets { get; init; } = new int[6];
}