using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace StarKindred.API.Utility;

public static class RelationshipHelper
{
    public static async Task<List<Relationship>> GetRelationships(Db db, List<Vassal> vassals, CancellationToken cToken)
    {
        var relationships = new List<Relationship>();
        
        for (int i = 0; i < vassals.Count - 1; i++)
        {
            for (int j = i + i; j < vassals.Count; j++)
            {
                var vassal1 = vassals[i];
                var vassal2 = vassals[j];

                relationships.Add(await GetRelationship(db, vassal1, vassal2, cToken));
            }
        }

        return relationships;
    }

    public sealed record DecorationResult(Vassal Vassal, Decoration Decoration, int Quintessence);

    public static async Task AdvanceRelationshipsWithNoChanceOfLoot(Db db, List<Vassal> vassals, int minutes, CancellationToken cToken)
    {
        if(vassals.Count == 0)
            return;

        var relationships = await GetRelationships(db, vassals, cToken);

        AdvanceRelationships(relationships, minutes, cToken);
    }
    
    public static async Task<DecorationResult?> AdvanceRelationshipsAndMaybeGetLoot(Db db, Random rng, List<Vassal> vassals, int minutes, CancellationToken cToken)
    {
        if(vassals.Count == 0)
            return null;

        var relationships = await GetRelationships(db, vassals, cToken);

        AdvanceRelationships(relationships, minutes, cToken);

        var maxLootChance = minutes / (60 * 24f);

        var totalRelationshipLevels = relationships.Sum(r => r.Level);

        var lootChance = maxLootChance * (1 - Math.Pow(0.9, totalRelationshipLevels));

        if (rng.NextDouble() >= lootChance)
            return null;

        var lootType = DecorationHelper.GetRandomDecoration(rng);

        var loot = await DecorationHelper.CollectDecoration(db, vassals.First().UserId, lootType, 1, cToken);

        var randomRelationshipWithLevels = rng.Next(relationships.Where(r => r.Level > 0).ToList());
        var randomVassal = rng.Next(randomRelationshipWithLevels.Vassals);

        var vassalsWithArtisticVision = vassals
            .Where(v => v.StatusEffects!.Any(se => se.Type == StatusEffectType.ArtisticVision))
            .ToList()
        ;

        var quintessence = 0;

        if (vassalsWithArtisticVision.Count > 0)
        {
            quintessence = 1000 * vassalsWithArtisticVision.Count;

            await ResourceHelper.CollectResources(db, vassals[0].UserId, new() { new(ResourceType.Quintessence, quintessence) }, cToken);

            vassalsWithArtisticVision.ForEach(v => StatusEffectsHelper.RemoveStatusEffect(v, StatusEffectType.ArtisticVision));
        }

        return new DecorationResult(randomVassal, loot, quintessence);
    }
    
    public static void AdvanceRelationships(List<Relationship> relationships, int minutes, CancellationToken cToken)
    {
        foreach (var relationship in relationships)
        {
            if(relationship.Level >= 5)
                continue;

            int percentBonus =  relationship.Vassals!.Count(v => v.Sign == AstrologicalSign.River) * 10;
            
            if(percentBonus > 0)
                minutes += minutes * percentBonus / 100;
                
            relationship.Minutes += minutes;

            while(relationship.Minutes > MinutesRequiredToLevel(relationship.Level) && relationship.Level < 5)
                relationship.Level++;
        }
    }

    public static int MinutesRequiredToLevel(int level)
        => (level + 1) * (level + 2) * 7 * 24 * 60 / 2;

    public static float LevelProgress(int level, int minutes)
    {
        var previousRequirement = MinutesRequiredToLevel(level - 1);
        var nextRequirement = MinutesRequiredToLevel(level);
        
        return (minutes - previousRequirement) / (float)(nextRequirement - previousRequirement);
    }
    
    private static async Task<Relationship> GetRelationship(Db db, Vassal vassal1, Vassal vassal2, CancellationToken cToken)
    {
        var relationship = await db.Relationships
            .Include(r => r.Vassals)
            .AsSingleQuery() // TODO: not profiled
            .FirstOrDefaultAsync(r => r.Vassals!.Any(v => v.Id == vassal1.Id) && r.Vassals!.Any(v => v.Id == vassal2.Id), cToken);

        if(relationship == null)
        {
            relationship = new Relationship
            {
                Vassals = new List<Vassal> { vassal1, vassal2 }
            };

            db.Relationships.Add(relationship);
        }

        return relationship;
    }
}