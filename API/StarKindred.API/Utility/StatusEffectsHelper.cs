using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;

namespace StarKindred.API.Utility;

public static class StatusEffectsHelper
{
    public static readonly StatusEffectType[] StatusEffectsKundravIsImmuneTo = new[]
    {
        StatusEffectType.BrokenBone
    };
    
    public static bool AddStatusEffect(Vassal vassal, StatusEffectType type, int duration)
    {
        if (vassal.Sign == AstrologicalSign.Kundrav && StatusEffectsKundravIsImmuneTo.Contains(type))
            return false;
        
        var statusEffect = vassal.StatusEffects!.FirstOrDefault(se => se.Type == type);
        
        if (statusEffect == null)
        {
            statusEffect = new StatusEffect()
            {
                Strength = duration,
                Type = type,
                VassalId = vassal.Id,
            };
            
            vassal.StatusEffects!.Add(statusEffect);
        }
        else
        {
            statusEffect.Strength += duration;
        }

        return true;
    }
    
    public static bool RemoveStatusEffect(Vassal vassal, StatusEffectType type)
    {
        var statusEffect = vassal.StatusEffects!.FirstOrDefault(se => se.Type == type);
        
        if (statusEffect == null)
            return false;
        
        vassal.StatusEffects!.Remove(statusEffect);
        
        return true;
    }

    public static readonly StatusEffectType[] SpecialDurationStatusEffects =
    {
        StatusEffectType.Focused,
        StatusEffectType.ArtisticVision,
    };

    public static void UpdateStatusEffects(Vassal vassal)
    {
        for(int i = vassal.StatusEffects!.Count - 1; i >= 0; i--)
        {
            var statusEffect = vassal.StatusEffects![i];

            if (SpecialDurationStatusEffects.Contains(statusEffect.Type))
                continue;
            
            statusEffect.Strength--;

            if (statusEffect.Strength <= 0)
                vassal.StatusEffects!.RemoveAt(i);
        }
    }
}