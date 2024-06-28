using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;

namespace StarKindred.API.Utility;

public static class RandomExtensions
{
    public static MissionOutcome NextMissionOutcome(this Random rng, List<Vassal> vassals)
    {
        var cannotCritFail = vassals.Any(v => v.Sign == AstrologicalSign.DoubleTrident);

        var result = rng.NextMissionOutcome(cannotCritFail);

        if (result != MissionOutcome.Bad && vassals.Any(v => StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.Focused)))
            result = MissionOutcome.Great;
        
        return result;
    }

    public static MissionOutcome NextMissionOutcome(this Random rng, Vassal vassal, bool cannotCritFail = false)
    {
        cannotCritFail = cannotCritFail || vassal.Sign == AstrologicalSign.DoubleTrident;

        var result = rng.NextMissionOutcome(cannotCritFail);

        if (result != MissionOutcome.Bad && StatusEffectsHelper.RemoveStatusEffect(vassal, StatusEffectType.Focused))
            result = MissionOutcome.Great;

        return result;
    }

    private static MissionOutcome NextMissionOutcome(this Random rng, bool cannotCritFail)
    {
        var roll = rng.Next(1, 21);

        if (roll == 1 && !cannotCritFail)
            return MissionOutcome.Bad;

        return roll == 20 ? MissionOutcome.Great : MissionOutcome.Good;
    }

}