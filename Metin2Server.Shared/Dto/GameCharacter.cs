using Metin2Server.Shared.Enums;

namespace Metin2Server.Shared.Dto;

public class GameCharacter : GameEntity
{
    public uint Vid { get; init; }
    public long AccountId { get; init; }
    public long CharacterId { get; init; }
    public string Name { get; init; } = string.Empty;
    public Empire Empire { get; init; }
    public byte SkillGroup { get; init; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public PersitencePoint PersitencePoints { get; set; } = new();
    public RuntimePoint RuntimePoints { get; set; } = new();
    public CharacterPkMode PkMode { get; set; } = CharacterPkMode.Peace;
    
    public class PersitencePoint
    {
        public Dictionary<CharacterPointType, int> Points { get; set; } = new();
        public Race Race { get; set; }
        public byte Voice { get; set; }
        public byte Level { get; set; }
        public uint Exp { get; set; }
        public uint Gold { get; set; }
        public int Hp { get; set; }
        public int Sp { get; set; }
        public int RandomHp { get; set; }
        public int RandomSp { get; set; }
        public int Stamina { get; set; }
        public byte SkillGroup { get; set; }
    }
    
    public class RuntimePoint
    {
        public Dictionary<CharacterPointType, int> Points { get; set; } = new();
        public Dictionary<CharacterPartType, byte> Parts { get; set; } = new();
        public float Rotation { get; set; }
        public int MaxHp { get; set; }
        public int MaxSp { get; set; }
        public byte BasePart { get; set; }
        public int MaxStamina { get; set; }
        public Dictionary<ushort, GameItem> Items { get; set; } = new();
    }
}