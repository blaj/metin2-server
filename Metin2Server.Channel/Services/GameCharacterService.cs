using Metin2Server.Shared.Application;
using Metin2Server.Shared.DbContracts.Common;
using Metin2Server.Shared.Dto;
using Metin2Server.Shared.Enums;
using Metin2Server.Shared.Extensions;
using Metin2Server.Shared.Utils;
using Microsoft.Extensions.Logging;
using Empire = Metin2Server.Shared.DbContracts.Common.Empire;

namespace Metin2Server.Channel.Services;

public class GameCharacterService
{
    private readonly CharacterVidAllocator _characterVidAllocator;
    private readonly GameCharacterStore _gameCharacterStore;
    private readonly ILogger<GameCharacterService> _logger;

    public GameCharacterService(
        ILogger<GameCharacterService> logger,
        CharacterVidAllocator characterVidAllocator,
        GameCharacterStore gameCharacterStore)
    {
        _characterVidAllocator = characterVidAllocator;
        _gameCharacterStore = gameCharacterStore;
        _logger = logger;
    }

    public GameCharacter? CreateGameCharacter(Character character, Empire empire, ISessionContext sessionContext)
    {
        if (sessionContext.AccountId == null)
        {
            return null;
        }

        var gameCharacterVid = _characterVidAllocator.Allocate();

        var gameCharacter = new GameCharacter
        {
            Vid = gameCharacterVid,
            AccountId = sessionContext.AccountId.Value,
            CharacterId = character.Id,
            Name = character.Name,
            Empire = empire.ToEntity(),
            SkillGroup = GrpcUtils.ToByteChecked(character.SkillGroup),
            X = character.Coordinates.X,
            Y = character.Coordinates.Y,
            Z = character.Coordinates.Z,
            PersitencePoints =
            {
                Race = character.Race.ToEntity(),
                Voice = 0,
                Level = (byte)character.Statistics.Level,
                Exp = character.Statistics.Exp,
                Gold = character.Statistics.Gold,
                Hp = character.Statistics.Hp,
                Sp = character.Statistics.Sp,
                RandomHp = character.Statistics.RandomHp,
                RandomSp = character.Statistics.RandomSp,
                Stamina = character.Statistics.Stamina,
                SkillGroup = (byte)character.SkillGroup
            }
        };
        
        SetPoint(gameCharacter, CharacterPointType.St, (int)character.Statistics.St);
        SetPoint(gameCharacter, CharacterPointType.Ht, (int)character.Statistics.Ht);
        SetPoint(gameCharacter, CharacterPointType.Dx, (int)character.Statistics.Dx);
        SetPoint(gameCharacter, CharacterPointType.Iq, (int)character.Statistics.Iq);

        SetPoint(gameCharacter, CharacterPointType.Stat, (int)character.Statistics.StatPoint);
        SetPoint(gameCharacter, CharacterPointType.Skill, (int)character.Statistics.SkillPoint);
        SetPoint(gameCharacter, CharacterPointType.SubSkill, (int)character.Statistics.SubSkillPoint);
        SetPoint(gameCharacter, CharacterPointType.HorseSkill, (int)character.Statistics.HorseSkillPoint);
        SetPoint(gameCharacter, CharacterPointType.LevelStep, (int)character.Statistics.LevelStep);
        SetPoint(gameCharacter, CharacterPointType.StatResetCount, (int)character.Statistics.StatResetCount);

        if (!_gameCharacterStore.TryAdd(gameCharacter))
        {
            _characterVidAllocator.Release(gameCharacterVid);
            return null;
        }

        ComputePoints(gameCharacter);

        sessionContext.GameCharacter = gameCharacter;
        return gameCharacter;
    }

    public void ComputePoints(GameCharacter gameCharacter)
    {
        var job = gameCharacter.PersitencePoints.Race.ToJob();
        var jobInitialPoints = job.GetInitialPoint();
        var persistencePoints = gameCharacter.PersitencePoints;

        SetPoint(gameCharacter, CharacterPointType.Stat, GetPoint(gameCharacter, CharacterPointType.Stat));
        SetPoint(gameCharacter, CharacterPointType.Skill, GetPoint(gameCharacter, CharacterPointType.Skill));
        SetPoint(gameCharacter, CharacterPointType.SubSkill, GetPoint(gameCharacter, CharacterPointType.SubSkill));
        SetPoint(gameCharacter, CharacterPointType.HorseSkill, GetPoint(gameCharacter, CharacterPointType.HorseSkill));
        SetPoint(gameCharacter, CharacterPointType.LevelStep, GetPoint(gameCharacter, CharacterPointType.LevelStep));
        SetPoint(gameCharacter, CharacterPointType.StatResetCount,
            GetPoint(gameCharacter, CharacterPointType.StatResetCount));

        SetPoint(gameCharacter, CharacterPointType.St, GetPoint(gameCharacter, CharacterPointType.St));
        SetPoint(gameCharacter, CharacterPointType.Ht, GetPoint(gameCharacter, CharacterPointType.Ht));
        SetPoint(gameCharacter, CharacterPointType.Dx, GetPoint(gameCharacter, CharacterPointType.Dx));
        SetPoint(gameCharacter, CharacterPointType.Iq, GetPoint(gameCharacter, CharacterPointType.Iq));

        SetPart(gameCharacter, CharacterPartType.Main, GetOriginalPart(gameCharacter, CharacterPartType.Main));
        SetPart(gameCharacter, CharacterPartType.Weapon, GetOriginalPart(gameCharacter, CharacterPartType.Weapon));
        SetPart(gameCharacter, CharacterPartType.Head, GetOriginalPart(gameCharacter, CharacterPartType.Head));
        SetPart(gameCharacter, CharacterPartType.Hair, GetOriginalPart(gameCharacter, CharacterPartType.Hair));

        SetPoint(gameCharacter, CharacterPointType.PartyAttackerBonus,
            GetPoint(gameCharacter, CharacterPointType.PartyAttackerBonus));
        SetPoint(gameCharacter, CharacterPointType.PartyTankerBonus,
            GetPoint(gameCharacter, CharacterPointType.PartyTankerBonus));
        SetPoint(gameCharacter, CharacterPointType.PartyBufferBonus,
            GetPoint(gameCharacter, CharacterPointType.PartyBufferBonus));
        SetPoint(gameCharacter, CharacterPointType.PartySkillMasterBonus,
            GetPoint(gameCharacter, CharacterPointType.PartySkillMasterBonus));
        SetPoint(gameCharacter, CharacterPointType.PartyHasteBonus,
            GetPoint(gameCharacter, CharacterPointType.PartyHasteBonus));
        SetPoint(gameCharacter, CharacterPointType.PartyDefenderBonus,
            GetPoint(gameCharacter, CharacterPointType.PartyDefenderBonus));

        SetPoint(gameCharacter, CharacterPointType.HpRecovery, GetPoint(gameCharacter, CharacterPointType.HpRecovery));
        SetPoint(gameCharacter, CharacterPointType.SpRecovery, GetPoint(gameCharacter, CharacterPointType.SpRecovery));

        SetPoint(gameCharacter, CharacterPointType.PcBangExpBonus,
            GetPoint(gameCharacter, CharacterPointType.PcBangExpBonus));
        SetPoint(gameCharacter, CharacterPointType.PcBangDropBonus,
            GetPoint(gameCharacter, CharacterPointType.PcBangDropBonus));

        var maxHp = 0;
        var maxSp = 0;
        var maxStamina = 0;

        // if (IsPC())
        // {
        maxHp = (int)(jobInitialPoints.MaxHp + persistencePoints.RandomHp +
                      GetPoint(gameCharacter, CharacterPointType.Ht) * jobInitialPoints.HpPerHt);
        maxSp = (int)(jobInitialPoints.MaxSp + persistencePoints.RandomSp +
                      GetPoint(gameCharacter, CharacterPointType.Iq) * jobInitialPoints.SpPerIq);
        maxStamina = jobInitialPoints.MaxStamina +
                     GetPoint(gameCharacter, CharacterPointType.Ht) * jobInitialPoints.StaminaPerCon;

        // {
        //     CSkillProto* pkSk = CSkillManager::instance().Get(SKILL_ADD_HP);
        //
        //     if (NULL != pkSk)
        //     {
        //         pkSk->SetPointVar("k", 1.0f * GetSkillPower(SKILL_ADD_HP) / 100.0f);
        //
        //         iMaxHP += static_cast<int>(pkSk->kPointPoly.Eval());
        //     }
        // }

        // �⺻ ����
        SetPoint(gameCharacter, CharacterPointType.MovSpeed, 100);
        SetPoint(gameCharacter, CharacterPointType.AttSpeed, 100);
        PointChange(gameCharacter, CharacterPointType.AttSpeed,
            GetPoint(gameCharacter, CharacterPointType.PartyHasteBonus), true, true);
        SetPoint(gameCharacter, CharacterPointType.CastingSpeed, 100);
        // }
        // else
        // {
        //     iMaxHP = m_pkMobData->m_table.dwMaxHP;
        //     iMaxSP = 0;
        //     iMaxStamina = 0;
        //
        //     SetPoint(POINT_ATT_SPEED, m_pkMobData->m_table.sAttackSpeed);
        //     SetPoint(POINT_MOV_SPEED, m_pkMobData->m_table.sMovingSpeed);
        //     SetPoint(POINT_CASTING_SPEED, m_pkMobData->m_table.sAttackSpeed);
        // }

        // if (IsPC())
        // {
        //     // �� Ÿ�� ���� ���� �⺻ ������ ���� ���� ���Ⱥ��� ������ ���� �����.
        //     // ���� ���� ���� ������ ���� �����̹Ƿ�, ����/������ ��ü ���� ����
        //     // ��ä������ �� �ö󰡰� �� ���̴�.
        //     if (GetMountVnum()) 
        //     {
        //         if (GetHorseST() > GetPoint(POINT_ST))
        //             PointChange(POINT_ST, GetHorseST() - GetPoint(POINT_ST));
        //
        //         if (GetHorseDX() > GetPoint(POINT_DX))
        //             PointChange(POINT_DX, GetHorseDX() - GetPoint(POINT_DX));
        //
        //         if (GetHorseHT() > GetPoint(POINT_HT))
        //             PointChange(POINT_HT, GetHorseHT() - GetPoint(POINT_HT));
        //
        //         if (GetHorseIQ() > GetPoint(POINT_IQ))
        //             PointChange(POINT_IQ, GetHorseIQ() - GetPoint(POINT_IQ));
        //     }
        //
        // }

        ComputeBattlePoints();

        if (maxHp != gameCharacter.RuntimePoints.MaxHp)
        {
            SetPersistencePoint(gameCharacter, CharacterPointType.MaxHp, maxHp);
        }

        PointChange(gameCharacter, CharacterPointType.MaxHp, 0, true, true);

        if (maxSp != gameCharacter.RuntimePoints.MaxSp)
        {
            SetPersistencePoint(gameCharacter, CharacterPointType.MaxSp, maxSp);
        }

        PointChange(gameCharacter, CharacterPointType.MaxSp, 0, true, true);

        SetPoint(gameCharacter, CharacterPointType.MaxStamina, maxStamina);

        // m_pointsInstant.dwImmuneFlag = 0;
        //
        // for (int i = 0 ; i < WEAR_MAX_NUM; i++) 
        // {
        //     LPITEM pItem = GetWear(i);
        //     if (pItem)
        //     {
        //         pItem->ModifyPoints(true);
        //         SET_BIT(m_pointsInstant.dwImmuneFlag, GetWear(i)->GetImmuneFlag());
        //     }
        // }

        // ��ȥ�� �ý���
        // ComputePoints������ �ɸ����� ��� �Ӽ����� �ʱ�ȭ�ϰ�,
        // ������, ���� � ���õ� ��� �Ӽ����� �����ϱ� ������,
        // ��ȥ�� �ý��۵� ActiveDeck�� �ִ� ��� ��ȥ���� �Ӽ����� �ٽ� ������Ѿ� �Ѵ�.
        // if (DragonSoul_IsDeckActivated())
        // {
        //     for (int i = WEAR_MAX_NUM + DS_SLOT_MAX * DragonSoul_GetActiveDeck(); 
        //          i < WEAR_MAX_NUM + DS_SLOT_MAX * (DragonSoul_GetActiveDeck() + 1); i++)	
        //     {
        //         LPITEM pItem = GetWear(i);
        //         if (pItem)
        //         {
        //             if (DSManager::instance().IsTimeLeftDragonSoul(pItem))
        //                 pItem->ModifyPoints(true);
        //         }
        //     }
        // }
        //
        // if (GetHP() > GetMaxHP())
        //     PointChange(POINT_HP, GetMaxHP() - GetHP());
        //
        // if (GetSP() > GetMaxSP())
        //     PointChange(POINT_SP, GetMaxSP() - GetSP());
        //
        // ComputeSkillPoints();
        //
        // RefreshAffect();
        // CPetSystem* pPetSystem = GetPetSystem();
        // if (NULL != pPetSystem)
        // {
        //     pPetSystem->RefreshBuff();
        // }
        //
        // for (TMapBuffOnAttrs::iterator it = m_map_buff_on_attrs.begin(); it != m_map_buff_on_attrs.end(); it++)
        // {
        //     it->second->GiveAllAttributes();
        // }
        //
        // UpdatePacket();
    }

    public void ComputeBattlePoints()
    {
    }

    public void CalculateMoveDuration()
    {
    }

    public void PointChange(
        GameCharacter gameCharacter,
        CharacterPointType pointType,
        int amount,
        bool hasAmount,
        bool broadcast)
    {
    }

    public int GetPoint(GameCharacter gameCharacter, CharacterPointType pointType)
    {
        var value = gameCharacter.RuntimePoints.Points.GetValueOrDefault(pointType, 0);
        var maxValue = int.MaxValue;

        if (pointType == CharacterPointType.StealHp || pointType == CharacterPointType.StealSp)
        {
            maxValue = 50;
        }

        if (value > maxValue)
        {
            _logger.LogError($"Character {gameCharacter.Name} has reached its maximum point ({value})");
        }

        return value;
    }

    public void SetPoint(GameCharacter gameCharacter, CharacterPointType pointType, int value)
    {
        gameCharacter.RuntimePoints.Points[pointType] = value;

        if (pointType == CharacterPointType.MovSpeed)
        {
            CalculateMoveDuration();
        }
    }

    public byte GetPart(GameCharacter gameCharacter, CharacterPartType partType)
    {
        return gameCharacter.RuntimePoints.Parts.GetValueOrDefault(partType, (byte)0);
    }

    public void SetPart(GameCharacter gameCharacter, CharacterPartType partType, byte value)
    {
        gameCharacter.RuntimePoints.Parts[partType] = value;
    }

    public byte GetOriginalPart(GameCharacter gameCharacter, CharacterPartType partType)
    {
        if (partType == CharacterPartType.Main)
        {
            // if (!IsPC()) // PC�� �ƴ� ��� ���� ��Ʈ�� �״�� ����
            //     return GetPart(PART_MAIN);
            return gameCharacter.RuntimePoints.BasePart;
        }

        if (partType == CharacterPartType.Head)
        {
            return GetPart(gameCharacter, CharacterPartType.Hair);
        }

        return 0;
    }

    public void SetPersistencePoint(GameCharacter gameCharacter, CharacterPointType pointType, int value)
    {
        gameCharacter.PersitencePoints.Points[pointType] = value;
    }
}