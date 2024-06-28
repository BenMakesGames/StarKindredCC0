using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Endpoints.Missions;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility.Missions;

public class Oracle
{
    public static async Task<Complete.ResponseDto> Do(
        Db db, Random rng, Vassal vassal, CancellationToken cToken
    )
    {
        var technologies = await db.UserTechnologies
            .Where(t => t.UserId == vassal.UserId && (t.Technology == TechnologyType.ShamanismI || t.Technology == TechnologyType.ShamanismII))
            .Select(t => t.Technology)
            .ToListAsync(cToken);

        var outcome = rng.NextMissionOutcome(vassal, technologies.Contains(TechnologyType.ShamanismI));
        
        string? words;
        
        var town = await db.Towns.FirstAsync(t => t.UserId == vassal.UserId, cToken);

        if (town.Level == 0)
        {
            var buildingCount = await db.Buildings.CountAsync(b => b.UserId == vassal.UserId, cToken);

            if (buildingCount < 4)
            {
                var remaining = 4 - buildingCount;
                var moreBuildings = remaining == 1 ? "more building" : "more buildings";
                
                words = $"Your people are few, and your leadership has not yet been proven. I will speak to you more when you have built {remaining} {moreBuildings}.";
            }
            else
            {
                words = "Your people are growing in numbers. The land is fertile and your people are well-fed. It is time to expand.\n\nI will speak to you more when you have upgraded a building to Level 10, and have at least 4 Vassals.";

                town.Level++;
            }
        }
        else if (town.Level == 1)
        {
            var hasALevel10Building = await db.Buildings.AnyAsync(
                b => b.UserId == vassal.UserId && b.Level >= 10,
                cToken
            );
            
            var vassalCount = await db.Vassals.CountAsync(v => v.UserId == vassal.UserId, cToken);

            if (!hasALevel10Building || vassalCount < 4)
            {
                words = "I will speak to you more when you have upgraded a building to Level 10, and have at least 4 Vassals.";
            }
            else
            {
                words = "It is time to build a Harbor. You will need to carry Stone from the mountains in the East. A bridge-builder has already begun work.\n\nGo. I will speak to you more when you have constructed a Harbor.";
                town.Level++;

                db.TownDecorations.Add(new()
                {
                    TownId = town.Id,
                    Type = DecorationType.WoodenBridge,
                    Scale = 130,
                    X = 67.1f,
                    Y = 55.0f,
                });

                db.TownDecorations.Add(new()
                {
                    TownId = town.Id,
                    Type = DecorationType.WoodenBridge,
                    Scale = 100,
                    X = 52.1f,
                    Y = 22.1f,
                });

                db.Decorations.Add(new()
                {
                    UserId = vassal.UserId,
                    Type = DecorationType.WoodenBridge,
                    Quantity = 2,
                });
            }
        }
        else if (town.Level == 2)
        {
            var hasAHarbor = await db.Buildings.AnyAsync(
                b => b.UserId == vassal.UserId && (b.Type == BuildingType.Harbor || b.Type == BuildingType.Fishery || b.Type == BuildingType.TradeDepot),
                cToken
            );

            if (!hasAHarbor)
            {
                words = "I will speak to you more when you have constructed a Harbor.";
            }
            else
            {
                words = "You have done well. I have nothing more to tell you.\n\nGood luck. Perhaps we will find each other in the next universe.";
                town.Level++;
            }
        }
        else
        {
            words = "Perhaps we will find each other in the next universe.";
        }

        var baseQuint = 10 + vassal.Level;

        if (technologies.Contains(TechnologyType.ShamanismII))
            baseQuint += baseQuint / 2;

        if (outcome == MissionOutcome.Great)
            baseQuint *= 2;

        var quint = baseQuint;

        var resourceQuantities = new List<ResourceQuantity>()
        {
            new(ResourceType.Quintessence, quint),
        };

        await ResourceHelper.CollectResources(
            db,
            vassal.UserId,
            resourceQuantities,
            cToken
        );

        var outcomeText = outcome == MissionOutcome.Great
            ? $"Great success! {vassal.Name} collected {quint} quintessence.\n\nThe Oracle had this to say:\n\n{words}"
            : $"{vassal.Name} collected {quint} quintessence.\n\nThe Oracle had this to say:\n\n{words}"
        ;

        if(outcome == MissionOutcome.Bad)
        {
            StatusEffectsHelper.AddStatusEffect(vassal, StatusEffectType.OracleTimeout, 3);
            
            outcomeText += $"\n\n{vassal.Name} accidentally insulted the Oracle, and was asked to leave.";
        }

        // reward Visionaries
        if(vassal.Nature == Nature.Visionary)
            VassalMath.IncreaseWillpower(vassal);

        return new Complete.ResponseDto(
            outcome,
            outcomeText,
            MissionReward.CreateFromResources(resourceQuantities)
        );
    }
}