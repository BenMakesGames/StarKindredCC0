using Microsoft.EntityFrameworkCore;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.API.Exceptions;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility;

public static class MissionMath
{
    public static int MinVassals(MissionType type, int level) => type switch
    {
        MissionType.BoatDate => 2,
        _ => 1,
    };

    public static int MaxVassals(MissionType type, int level) => type switch
    {
        MissionType.Oracle => 1,
        MissionType.RecruitTown => 2,
        MissionType.HuntLevel0 => 2,
        MissionType.HuntLevel10 => 2,
        MissionType.HuntLevel20 => 2,
        MissionType.HuntLevel50 => 3,
        MissionType.HuntLevel80 => 3,
        MissionType.HuntLevel120 => 3,
        MissionType.HuntLevel200 => 4,

        MissionType.Settlers => 2,
        MissionType.TreasureHunt => 2 + level / 100,
        MissionType.WanderingMonster => 2 + level / 100,

        MissionType.BoatDate => 2,
            
        _ => throw new ArgumentException("Unrecognized mission type", nameof(type))
    };

    public static int DurationInMinutes(int minutes, List<Vassal> vassals)
    {
        var multiplier = 1.0;

        foreach(var v in vassals)
        {
            if(v.Sign == AstrologicalSign.PapyrusBoat)
                multiplier *= 0.9;

            multiplier *= WeaponHelper.FasterMissionsMultiplier(v.Weapon);
        }

        return (int)(minutes * multiplier);
    }

    public static int DurationInMinutes(MissionType mission, int missionLevel, List<Vassal> vassals)
    {
        var baseTime = mission switch
        {
            MissionType.Oracle => 60,
            MissionType.RecruitTown => 480,
            MissionType.HuntLevel0 => 60,
            MissionType.HuntLevel10 => 90,
            MissionType.HuntLevel20 => 120,
            MissionType.HuntLevel50 => 240,
            MissionType.HuntLevel80 => 360,
            MissionType.HuntLevel120 => 480,
            MissionType.HuntLevel200 => 720,
            
            MissionType.Settlers => 360 + missionLevel * 2,
            MissionType.WanderingMonster => 60 + missionLevel * 33 / 10,
            MissionType.TreasureHunt => 60 + missionLevel * 33 / 10,

            MissionType.BoatDate => 60,
            
            _ => throw new ArgumentException("Unrecognized mission type", nameof(mission))
        };

        return DurationInMinutes(baseTime, vassals);
    }

    public static List<MissionType> AvailableMissions(int vassalCount, int highestVassalLevel)
    {
        var list = new List<MissionType>()
        {
            MissionType.Oracle,
            MissionType.RecruitTown,
        };

        if(vassalCount < 3)
            return list;

        list.Add(MissionType.HuntLevel0);

        if(highestVassalLevel < 5)
            return list;
        
        list.Add(MissionType.HuntLevel10);

        if(highestVassalLevel < 15)
            return list;
        
        list.Add(MissionType.HuntLevel20);

        if(highestVassalLevel < 40)
            return list;
        
        list.Add(MissionType.HuntLevel50);

        if(highestVassalLevel < 60)
            return list;
        
        list.Add(MissionType.HuntLevel80);

        if(highestVassalLevel < 80)
            return list;
        
        list.Add(MissionType.HuntLevel120);

        if(highestVassalLevel < 100)
            return list;
        
        list.Add(MissionType.HuntLevel200);

        return list;
    }
    
    public static int PercentChanceOfSuccess(int monsterLevel, Element monsterElement, int maxVassals, List<Vassal> vassals)
    {
        var totalVassalLevel = vassals.Sum(v =>
        {
            var levelBonus =
                WeaponHelper.BonusHuntingLevels(v.Weapon) +
                (v.StatusEffects!.Any(se => se.Type == StatusEffectType.Power) ? 5 : 0)
            ;
            
            if (v.Element.IsStrongAgainst(monsterElement))
                return ((v.Level + levelBonus) * 3 / 2) + 1;
            
            if (v.Element.IsWeakAgainst(monsterElement))
                return ((v.Level + levelBonus) * 1 / 2) + 1;

            return v.Level + levelBonus + 1;
        });
        
        // success chance is either:
        // 10%, 50%, 80%, or 100%
        
        float fraction = (float)totalVassalLevel / (monsterLevel + maxVassals);

        return fraction switch
        {
            >= 1.5f => 100,
            >= 1 => 80,
            >= 0.5f => 50,
            _ => 10
        };
    }
    
    public static int PercentChanceOfSuccess(int challengeLevel, int maxVassals, List<Vassal> vassals, bool applyHuntingBonus)
    {
        var totalVassalLevel = applyHuntingBonus
            ? vassals.Sum(v =>
                WeaponHelper.BonusHuntingLevels(v.Weapon) +
                (v.StatusEffects!.Any(se => se.Type == StatusEffectType.Power) ? 5 : 0) +
                v.Level +
                1
            )
            : vassals.Sum(v => v.Level + 1)
        ;
        
        // success chance is either:
        // 10%, 50%, 80%, or 100%
        
        float fraction = (float)totalVassalLevel / (challengeLevel + maxVassals);

        return fraction switch
        {
            >= 1.5f => 100,
            >= 1 => 80,
            >= 0.5f => 50,
            _ => 10
        };
    }

    public static void ValidateVassalStatusEffects(MissionType type, List<Vassal> vassals)
    {
        if(IsAnimalMonsterOrTreasureHunt(type) && vassals.Any(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.BrokenBone)))
            throw new UnprocessableEntity("Vassals with broken bones cannot participate in animal-hunts, monster-hunts, or treasure-hunts.");
        
        if(type == MissionType.Oracle && vassals.Any(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.OracleTimeout)))
            throw new UnprocessableEntity("Vassals which have recently offended The Oracle cannot go to see The Oracle...");
    }

    public static bool IsAnimalMonsterOrTreasureHunt(MissionType type) => type is
        MissionType.WanderingMonster or
        MissionType.TreasureHunt or
        MissionType.HuntLevel0 or
        MissionType.HuntLevel10 or
        MissionType.HuntLevel20 or
        MissionType.HuntLevel50 or
        MissionType.HuntLevel80 or
        MissionType.HuntLevel120 or
        MissionType.HuntLevel200;

    public static void UpdateVassalsAfterMissionCompletion(List<Vassal> vassals, MissionType missionType, MissionOutcome outcome, bool missionCouldHaveYieldedWeapon)
    {
        foreach(var v in vassals)
        {
            if(v.Nature == Nature.Perfectionist)
            {
                if (outcome == MissionOutcome.Great)
                    VassalMath.IncreaseWillpower(v);
            }
            else if(v.Nature == Nature.Loner)
            {
                if(vassals.Count == 1 && missionType != MissionType.Oracle)
                    VassalMath.IncreaseWillpower(v);
            }
            else if(v.Nature == Nature.Competitor)
            {
                if(vassals.Count > 1 && v.Level > vassals.Where(v2 => v2.Id != v.Id).Select(v2 => v2.Level).Max())
                    VassalMath.IncreaseWillpower(v);
            }

            v.MissionId = null;
            v.TimedMissionId = null;
            
            StatusEffectsHelper.UpdateStatusEffects(v);

            if (v.Weapon != null)
                WeaponHelper.DegradeWeaponDurability(v, v.Weapon, missionType, missionCouldHaveYieldedWeapon);
        }
    }

    public static int AttackDamage(IList<Vassal> vassals, Element element) => vassals.Sum(v =>
    {
        var effectiveLevel =
            v.Level +
            WeaponHelper.BonusHuntingLevels(v.Weapon) +
            (v.StatusEffects!.Any(se => se.Type == StatusEffectType.Power) ? 5 : 0)
        ;

        var damage = (effectiveLevel + 5) * (effectiveLevel + 5) / 10;

        if (v.Element.IsStrongAgainst(element))
            return damage * 3 / 2;

        if (v.Element.IsWeakAgainst(element))
            return damage * 1 / 2;

        return damage;
    });

    private static readonly TechnologyType[] TrackingTechnologies =
    {
        TechnologyType.TrackingI, TechnologyType.TrackingII, TechnologyType.TrackingIII
    };

    public static async Task<int> MaxRumors(Db db, Guid userId, CancellationToken cToken)
    {
        var trackingTechnologies = await db.UserTechnologies
            .CountAsync(t => t.UserId == userId && TrackingTechnologies.Contains(t.Technology), cToken);

        return 3 + trackingTechnologies;
    }
}