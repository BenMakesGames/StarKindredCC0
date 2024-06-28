using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.API.Utility.Buildings;
using StarKindred.API.Utility.Technologies;

namespace StarKindred.API.Endpoints.Towns;

[ApiController]
public sealed class My
{
    private const int MaxGoodiesPerTown = 12;
    private const int GoodiePositions = 18;

    private static readonly TechnologyType[] AstrologyTechnologies =
    {
        TechnologyType.AstrologyI,
        TechnologyType.AstrologyII,
        TechnologyType.AstrologyIII,
        TechnologyType.AstrologyIV
    };
    
    [HttpGet("/towns/my")]
    public async Task<ApiResponse<Response>> _(
        [FromServices] ICurrentUser currentUser, [FromServices] Db db,
        [FromServices] Random rng,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);

        var town = await db.Towns
            .Include(t => t.Decorations)
            .FirstAsync(t => t.UserId == session.UserId, cToken);
        
        var buildingEntities = await db.Buildings
            .Where(b => b.UserId == session.UserId)
            .ToListAsync(cToken);

        var technologies = await db.UserTechnologies
            .Where(t => t.UserId == session.UserId)
            .Select(t => t.Technology)
            .ToListAsync(cToken);

        var hasArchitectureI = technologies.Contains(TechnologyType.ArchitectureI);
        var hasArchitectureII = technologies.Contains(TechnologyType.ArchitectureII);
        var hasArchitectureIII = technologies.Contains(TechnologyType.ArchitectureIII);
        var hasExpansion = technologies.Contains(TechnologyType.Expansion);

        var buildings = buildingEntities
            .Select(b => new BuildingDto(
                b.Id,
                b.Position,
                b.Type,
                b.Level,
                BuildingCosts.MaxLevel(b.Type, hasExpansion),
                BuildingHarvestMath.SecondsTowardNextYield(b.Type, b.Level, b.LastHarvestedOn),
                BuildingHarvestMath.MinutesForYield(b.Type, b.Level) * 60,
                BuildingHarvestMath.GetYield(b.Type, b.Level, b.LastHarvestedOn, technologies)
                    .Resources
                    .Where(r => r.Quantity > 0)
                    .Select(r => new ResourceQuantity(r.Type, r.Quantity))
                    .ToList(),
                BuildingCosts.CostToUpgrade(b.Type, b.Level, hasArchitectureI, hasArchitectureII, hasExpansion),
                b.Level == 10 && BuildingCosts.MaxLevel(b.Type, hasExpansion) == 10 ? (hasArchitectureIII ? BuildingCosts.BuildingsAvailableAtPosition(b.Position).SelectMany(BuildingHarvestMath.GetAvailableSpecializations).ToList() : BuildingHarvestMath.GetAvailableSpecializations(b.Type)) : new(),
                b.Level >= 20 ? b.PowerLastActivatedOn.AddDays(1) : null,
                b.Level >= 20 ? BuildingPowerHelpers.AvailablePowers(b, technologies) : null
            ))
            .ToList()
        ;
        
        var resources = await db.Resources
            .Where(r => r.UserId == session.UserId)
            .Select(r => new ResourceQuantity(r.Type, r.Quantity))
            .ToListAsync(cToken);

        // maybe add goodies last
        var goodies = await db.Goodies.Where(g => g.UserId == session.UserId).ToListAsync(cToken);
        
        var numberOfGoodies = (int)((DateTimeOffset.UtcNow - town.LastGoodie).TotalMinutes / 60);

        if (numberOfGoodies > 0)
            goodies = await AddGoodies(db, rng, session.UserId, goodies, numberOfGoodies, town, technologies, cToken);

        var decorationTechsResearched = technologies.Count(TechTree.DecorationTechs.Contains);
        var maxDecorations = 20 + decorationTechsResearched * (decorationTechsResearched + 1);

        return new(new Response(
            town.Name,
            town.Level,
            town.NextRumor <= DateTimeOffset.UtcNow,
            town.CanDecorate,
            maxDecorations,
            buildings,
            town.Decorations!.Select(d => new DecorationDto(d.Type, d.X, d.Y, d.Scale, d.FlipX)).ToList(),
            goodies.Select(g => new GoodieDto(g.Location, g.Type, g.Quantity)).ToList(),
            resources
        ));
    }

    private static async Task<List<Common.Entities.Db.Goodie>> AddGoodies(
        Db db,
        Random rng,
        Guid userId,
        List<Common.Entities.Db.Goodie> goodies,
        int numberOfGoodies,
        Town town,
        List<TechnologyType> technologies,
        CancellationToken cToken
    )
    {
        var goodiesToAdd = Math.Min(numberOfGoodies, MaxGoodiesPerTown) - goodies.Count;

        if (goodiesToAdd < 0)
            return goodies;
        
        var now = DateTimeOffset.UtcNow;
        town.LastGoodie = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, now.Offset);
        
        var availableLocations = Enumerable.Range(0, GoodiePositions)
            .Except(goodies.Select(g => g.Location))
            .ToList();

        var possibleGoodies = town.Level switch
        {
            0 => new[] { ResourceType.Wood, ResourceType.Wood, ResourceType.Wood, ResourceType.Gold, ResourceType.Wheat },
            1 => new[] { ResourceType.Wood, ResourceType.Wood, ResourceType.Gold, ResourceType.Wheat, ResourceType.Meat },
            2 => new[] { ResourceType.Wood, ResourceType.Gold, ResourceType.Wheat, ResourceType.Meat, ResourceType.Stone },
            _ => Enum.GetValues<ResourceType>().ToArray(),
        };
        
        availableLocations.Shuffle(rng);

        var astrologyCount = technologies.Count(AstrologyTechnologies.Contains);

        for (var i = 0; i < goodiesToAdd; i++)
        {
            var type = rng.Next(possibleGoodies);
            var quantity = rng.Next(3, rng.Next(4, 10 + 1) + 1);

            if (type == ResourceType.Quintessence)
                quantity *= (astrologyCount + 1);

            var goodie = new Common.Entities.Db.Goodie()
            {
                UserId = userId,
                Location = availableLocations[0],
                Type = type,
                Quantity = quantity,
            };

            availableLocations.RemoveAt(0);
            
            db.Goodies.Add(goodie);
            goodies.Add(goodie);
        }

        await db.SaveChangesAsync(cToken);

        return goodies;
    }

    public sealed record Response(string Name, int Level, bool RumorWaiting, bool CanDecorate, int MaxDecorations, List<BuildingDto> Buildings, List<DecorationDto> Decorations, List<GoodieDto> Goodies, List<ResourceQuantity> Resources);
    public sealed record BuildingDto(Guid Id, int Position, BuildingType Type, int Level, int MaxLevel, int YieldProgress, int SecondsRequired, List<ResourceQuantity> Yield, List<ResourceQuantity>? UpgradeCost, List<BuildingType> AvailableSpecializations, DateTimeOffset? PowersAvailableOn, List<PowerDto>? PowersAvailable);
    public sealed record GoodieDto(int Position, ResourceType Type, int Quantity);
    public sealed record DecorationDto(DecorationType Type, float X, float Y, int Scale, bool FlipX);
}