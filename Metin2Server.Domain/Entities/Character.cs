using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Metin2Server.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Metin2Server.Domain.Entities;

[Table("character", Schema = "character")]
public class Character : AuditingEntity
{
    [Required]
    [StringLength(24, MinimumLength = 3)]
    public required string Name { get; set; }

    public required Race Race { get; set; }

    public byte Index { get; set; } = 0;
    
    public byte PartBase { get; set; } = 0;

    public ushort PartMain { get; set; } = 0;

    public ushort PartHair { get; set; } = 0;

    public byte SkillGroup { get; set; } = 0;

    public CoordinatesInfo Coordinates { get; set; } = new();

    public StatisticsInfo Statistics { get; set; } = new();

    public HorseStatisticsInfo HorseStatistics { get; set; } = new();

    public List<CharacterSkill> Skills { get; set; } = [];

    public List<CharacterQuickSlot> QuickSlots { get; set; } = [];

    public required Account Account { get; set; }
    
    public List<CharacterItem> Items { get; set; } = [];

    [Owned]
    public class CoordinatesInfo
    {
        public byte Dir { get; set; } = 0;

        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public int Z { get; set; } = 0;

        public uint? MapIndex { get; set; } = null;

        public int? ExitX { get; set; } = null;

        public int? ExitY { get; set; } = null;

        public uint? ExitMapIndex { get; set; } = null;
    }

    [Owned]
    public class StatisticsInfo
    {
        public int Hp { get; set; } = 0;

        public int Sp { get; set; } = 0;

        public int Stamina { get; set; } = 0;

        public int RandomHp { get; set; } = 0;

        public int RandomSp { get; set; } = 0;

        public uint PlayTime { get; set; } = 0;

        public byte Level { get; set; } = 1;

        public byte LevelStep { get; set; } = 0;

        public ushort St { get; set; } = 0;

        public ushort Ht { get; set; } = 0;

        public ushort Dx { get; set; } = 0;

        public ushort Iq { get; set; } = 0;

        public uint Exp { get; set; } = 0;

        public uint Gold { get; set; } = 0;

        public ushort StatPoint { get; set; } = 0;

        public ushort SkillPoint { get; set; } = 0;

        public ushort SubSkillPoint { get; set; } = 0;

        public ushort HorseSkillPoint { get; set; } = 0;

        public int Alignment { get; set; } = 0;

        public ushort StatResetCount { get; set; } = 0;
    }

    [Owned]
    public class HorseStatisticsInfo
    {
        public byte Level { get; set; } = 0;

        public bool Riding { get; set; } = false;

        public short Stamina { get; set; } = 0;

        public short Health { get; set; } = 0;

        public int HealthDropTime { get; set; } = 0;
    }
}