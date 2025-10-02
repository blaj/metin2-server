using Metin2Server.Domain.Entities;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Utils;

namespace Metin2Server.Domain.Extensions;

public static class CharacterExtensions
{
    public static Shared.DbContracts.Common.Character ToProto(this Character entity)
    {
        if (entity == null)
        {
            return null!;
        }

        return new Shared.DbContracts.Common.Character
        {
            Id = entity.Id,
            Name = entity.Name ?? string.Empty,
            Race = entity.Race.ToProto(),
            Index = entity.Index,
            PartBase = entity.PartBase,
            PartMain = entity.PartMain,
            PartHair = entity.PartHair,
            SkillGroup = entity.SkillGroup,
            Coordinates = entity.Coordinates.ToProto(),
            Statistics = entity.Statistics.ToProto(),
            HorseStatistics = entity.HorseStatistics.ToProto()
        };
    }

    public static Character ToEntity(this Shared.DbContracts.Common.Character dto, Account account)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (account == null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        return new Character
        {
            Name = dto.Name ?? string.Empty,
            Race = dto.Race.ToEntity(),
            Index = GrpcUtils.ToByteChecked(dto.Index),
            PartBase = GrpcUtils.ToByteChecked(dto.PartBase),
            PartMain = GrpcUtils.ToUShortChecked(dto.PartMain),
            PartHair = GrpcUtils.ToUShortChecked(dto.PartHair),
            SkillGroup = GrpcUtils.ToByteChecked(dto.SkillGroup),
            Coordinates = dto.Coordinates.ToEntity(),
            Statistics = dto.Statistics.ToEntity(),
            HorseStatistics = dto.HorseStatistics.ToEntity(),
            Account = account
        };
    }

    public static Shared.DbContracts.Common.CoordinatesInfo ToProto(this Character.CoordinatesInfo value)
    {
        if (value == null)
        {
            return null!;
        }

        return new Shared.DbContracts.Common.CoordinatesInfo
        {
            Dir = value.Dir,
            X = value.X,
            Y = value.Y,
            Z = value.Z,
            MapIndex = value.MapIndex ?? 0u,
            ExitX = value.ExitX ?? 0,
            ExitY = value.ExitY ?? 0,
            ExitMapIndex = value.ExitMapIndex ?? 0u
        };
    }

    public static Character.CoordinatesInfo ToEntity(this Shared.DbContracts.Common.CoordinatesInfo dto)
    {
        if (dto == null)
        {
            return new Character.CoordinatesInfo();
        }

        return new Character.CoordinatesInfo
        {
            Dir = GrpcUtils.ToByteChecked(dto.Dir),
            X = dto.X,
            Y = dto.Y,
            Z = dto.Z,
            MapIndex = dto.MapIndex == 0 ? null : dto.MapIndex,
            ExitX = dto.ExitX == 0 ? null : dto.ExitX,
            ExitY = dto.ExitY == 0 ? null : dto.ExitY,
            ExitMapIndex = dto.ExitMapIndex == 0 ? null : dto.ExitMapIndex
        };
    }

    public static Shared.DbContracts.Common.StatisticsInfo ToProto(this Character.StatisticsInfo value)
    {
        if (value is null) return null!;

        return new Shared.DbContracts.Common.StatisticsInfo
        {
            Hp = value.Hp,
            Sp = value.Sp,
            Stamina = value.Stamina,
            RandomHp = value.RandomHp,
            RandomSp = value.RandomSp,
            PlayTime = value.PlayTime,
            Level = value.Level,
            LevelStep = value.LevelStep,
            St = value.St,
            Ht = value.Ht,
            Dx = value.Dx,
            Iq = value.Iq,
            Exp = value.Exp,
            Gold = value.Gold,
            StatPoint = value.StatPoint,
            SkillPoint = value.SkillPoint,
            SubSkillPoint = value.SubSkillPoint,
            HorseSkillPoint = value.HorseSkillPoint,
            Alignment = value.Alignment,
            StatResetCount = value.StatResetCount
        };
    }

    public static Character.StatisticsInfo ToEntity(this Shared.DbContracts.Common.StatisticsInfo dto)
    {
        if (dto == null)
        {
            return new Character.StatisticsInfo();
        }

        return new Character.StatisticsInfo
        {
            Hp = dto.Hp,
            Sp = dto.Sp,
            Stamina = dto.Stamina,
            RandomHp = dto.RandomHp,
            RandomSp = dto.RandomSp,
            PlayTime = dto.PlayTime,
            Level = GrpcUtils.ToByteChecked(dto.Level),
            LevelStep = GrpcUtils.ToByteChecked(dto.LevelStep),
            St = GrpcUtils.ToUShortChecked(dto.St),
            Ht = GrpcUtils.ToUShortChecked(dto.Ht),
            Dx = GrpcUtils.ToUShortChecked(dto.Dx),
            Iq = GrpcUtils.ToUShortChecked(dto.Iq),
            Exp = dto.Exp,
            Gold = dto.Gold,
            StatPoint = GrpcUtils.ToUShortChecked(dto.StatPoint),
            SkillPoint = GrpcUtils.ToUShortChecked(dto.SkillPoint),
            SubSkillPoint = GrpcUtils.ToUShortChecked(dto.SubSkillPoint),
            HorseSkillPoint = GrpcUtils.ToUShortChecked(dto.HorseSkillPoint),
            Alignment = dto.Alignment,
            StatResetCount = GrpcUtils.ToUShortChecked(dto.StatResetCount)
        };
    }

    public static Shared.DbContracts.Common.HorseStatisticsInfo ToProto(this Character.HorseStatisticsInfo value)
    {
        if (value == null)
        {
            return null!;
        }

        return new Shared.DbContracts.Common.HorseStatisticsInfo
        {
            Level = value.Level,
            Riding = value.Riding,
            Stamina = value.Stamina,
            Health = value.Health,
            HealthDropTime = value.HealthDropTime
        };
    }

    public static Character.HorseStatisticsInfo ToEntity(this Shared.DbContracts.Common.HorseStatisticsInfo dto)
    {
        if (dto == null)
        {
            return new Character.HorseStatisticsInfo();
        }

        return new Character.HorseStatisticsInfo
        {
            Level = GrpcUtils.ToByteChecked(dto.Level),
            Riding = dto.Riding,
            Stamina = GrpcUtils.ToShortChecked(dto.Stamina),
            Health = GrpcUtils.ToShortChecked(dto.Health),
            HealthDropTime = dto.HealthDropTime
        };
    }
}