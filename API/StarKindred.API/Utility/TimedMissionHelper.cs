using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace StarKindred.API.Utility;

public static class TimedMissionHelper
{
    private static readonly int[] LandLocations = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private static readonly int[] SeaLocations = { 10, 11, 12 };

    public static readonly Species[] SettlerSpecies =
    {
        Species.Human,
        Species.Midine,
        Species.Ruqu
    };

    public static async Task<List<int>> GetAvailableLandLocations(Db db, Guid userId, CancellationToken cToken)
    {
        var takenLocations = await db.TimedMissions
            .Where(m => m.UserId == userId)
            .Select(m => m.Location)
            .ToListAsync(cToken);

        return LandLocations.Where(l => !takenLocations.Contains(l)).ToList();
    }

    public static async Task<List<int>> GetAvailableSeaLocations(Db db, Guid userId, CancellationToken cToken)
    {
        var takenLocations = await db.TimedMissions
            .Where(m => m.UserId == userId)
            .Select(m => m.Location)
            .ToListAsync(cToken);

        return SeaLocations.Where(l => !takenLocations.Contains(l)).ToList();
    }

    public static async Task<int> GetMissionLevel(Db db, Random rng, Guid userId, CancellationToken cToken)
    {
        var maxVassalLevel = await db.Vassals.Where(v => v.UserId == userId).MaxAsync(v => v.Level, cToken);
        var averageVassalLevel = (int)(await db.Vassals.Where(v => v.UserId == userId).AverageAsync(v => v.Level, cToken));

        return rng.Next(averageVassalLevel * 2 / 3, maxVassalLevel * 2 + 1);
    }

    public static TimedMission CreateBoatDate(Guid userId) => new()
    {
        UserId = userId,
        Type = MissionType.BoatDate,
        Description = "The townsfolk have arranged a scenic boat tour for your Vassals."
    };

    public static TimedMission CreateSettlersMission(Random rng, Guid userId, int level) => new()
    {
        UserId = userId,
        Level = Math.Clamp(level, 0, 100),
        Species = rng.Next(SettlerSpecies),
        Type = MissionType.Settlers,
        Description = "Settlers have been spotted crossing the region. Perhaps we could invite them to join us."
    };

    public static TimedMission CreateTreasureHunt(Random rng, Guid userId, int level)
    {
        var isWeapon = rng.Next(4) != 1;

        return new TimedMission()
        {
            UserId = userId,
            Level = level,
            Weapon = isWeapon ? rng.NextEnumValue<WeaponBonus>() : null,
            Treasure = isWeapon ? null : RandomTreasure(rng),
            Type = MissionType.TreasureHunt,
            Description = "Rumors speak of ancient ruins filled with treasure!"
        };
    }

    private static TreasureType RandomTreasure(Random rng) => rng.Next(new[]
    {
        TreasureType.GoldChest,
        TreasureType.GoldChest,
        TreasureType.GoldChest,
        TreasureType.RubyChest,
        TreasureType.CupOfLife,
        TreasureType.WeaponChest,
        TreasureType.TwilightChest,
    });

    public static TimedMission CreateWanderingMonster(Random rng, Guid userId, int level)
    {
        var isWeapon = rng.Next(4) == 1;

        return new TimedMission()
        {
            UserId = userId,
            Level = level,
            Weapon = isWeapon ? rng.NextEnumValue<WeaponBonus>() : null,
            Treasure = isWeapon ? null : RandomTreasure(rng),
            Element = rng.NextEnumValue<Element>(),
            Type = MissionType.WanderingMonster,
            Description = "A terrible creature has been spotted roaming the country..."
        };
    }

}