using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Technologies;

public sealed record TechInfo(TechnologyType? PreReq, TownLeaderPosition Category, string Title, int Level);

public static class TechTree
{
    public static readonly TechnologyType[] DecorationTechs =
    {
        TechnologyType.TownBeautification,
        TechnologyType.PublicArtI,
        TechnologyType.PublicArtII,
        TechnologyType.PublicArtIII,
        TechnologyType.PublicArtIV
    };

    private const int OneDay = 24 * 60;
    
    // gettin' fibbonacci up in here:
    public static int ResearchTime(this TechInfo tech, int leaderLevel)
    {
        var baseTime = tech.Level switch
        {
            1 => OneDay * 2,
            2 => OneDay * 3,
            3 => OneDay * 5,
            4 => OneDay * 8,
            5 => OneDay * 13,
            6 => OneDay * 21,
            _ => throw new Exception($"Unsupported tech level: {tech.Level}")
        };

        return (int)Math.Ceiling(baseTime - baseTime * leaderLevel * 2 / 300.0);
    }

    public static List<ResourceQuantity> ResearchCost(this TechInfo tech, Nature leaderNature)
    {
        var costMultiplier = tech.Level switch
        {
            1 => 1,
            2 => 2,
            3 => 3,
            4 => 5,
            5 => 8,
            6 => 11,
            _ => throw new Exception($"Unsupported tech level: {tech.Level}")
        };

        var mainResourceType = VassalMath.NatureResourceType(leaderNature);

        if (mainResourceType == ResourceType.Gold)
        {
            return new List<ResourceQuantity>()
            {
                new(ResourceType.Gold, 50 * tech.Level + 1000 * costMultiplier)
            };
        }
        else
        {
            return new List<ResourceQuantity>()
            {
                new(ResourceType.Gold, 50 * tech.Level),
                new(mainResourceType, 1000 * costMultiplier)
            };
        }
    }

    public static readonly Dictionary<TechnologyType, TechInfo> TechInfo = new()
    {
        // blacksmithing:
        {
            TechnologyType.Blacksmithing,
            new(null, TownLeaderPosition.Economy, "Blacksmithing", 1)
        },
        {
            TechnologyType.CupronickleI,
            new(TechnologyType.Blacksmithing, TownLeaderPosition.Economy, "Cupronickle I", 2)
        },
        {
            TechnologyType.CupronickleII,
            new(TechnologyType.CupronickleI, TownLeaderPosition.Economy, "Cupronickle II", 3)
        },
        {
            TechnologyType.CupronickleIII,
            new(TechnologyType.CupronickleII, TownLeaderPosition.Economy, "Cupronickle III", 4)
        },
        {
            TechnologyType.HushingI,
            new(TechnologyType.Blacksmithing, TownLeaderPosition.Economy, "Hushing I", 2)
        },
        {
            TechnologyType.HushingII,
            new(TechnologyType.HushingI, TownLeaderPosition.Economy, "Hushing II", 3)
        },
        {
            TechnologyType.ScrappingI,
            new(TechnologyType.Blacksmithing, TownLeaderPosition.Economy, "Scrapping I", 3)
        },
        {
            TechnologyType.ScrappingII,
            new(TechnologyType.ScrappingI, TownLeaderPosition.Economy, "Scrapping II", 5)
        },

        // woodworking:
        {
            TechnologyType.WoodWorking,
            new(null, TownLeaderPosition.Economy, "Woodworking", 1)
        },
        {
            TechnologyType.ArchitectureI,
            new(TechnologyType.WoodWorking, TownLeaderPosition.Economy, "Architecture I", 2)
        },
        {
            TechnologyType.ArchitectureII,
            new(TechnologyType.ArchitectureI, TownLeaderPosition.Economy, "Architecture II", 3)
        },
        {
            TechnologyType.ArchitectureIII,
            new(TechnologyType.ArchitectureII, TownLeaderPosition.Economy, "Architecture III", 4)
        },
        {
            TechnologyType.FinishingI,
            new(TechnologyType.WoodWorking, TownLeaderPosition.Economy, "Finishing I", 2)
        },
        {
            TechnologyType.FinishingII,
            new(TechnologyType.FinishingI, TownLeaderPosition.Economy, "Finishing II", 3)
        },
        {
            TechnologyType.FinishingIII,
            new(TechnologyType.FinishingII, TownLeaderPosition.Economy, "Finishing III", 4)
        },

        // freetrade:
        {
            TechnologyType.FreeTrade,
            new(null, TownLeaderPosition.Economy, "Merchant's Guild", 1)
        },
        {
            TechnologyType.MapMakingI,
            new(TechnologyType.FreeTrade, TownLeaderPosition.Economy, "Mapmaking I", 2)
        },
        {
            TechnologyType.MapMakingII,
            new(TechnologyType.MapMakingI, TownLeaderPosition.Economy, "Mapmaking II", 3)
        },
        {
            TechnologyType.MapMakingIII,
            new(TechnologyType.MapMakingII, TownLeaderPosition.Economy, "Mapmaking III", 4)
        },
        {
            TechnologyType.MapMakingIV,
            new(TechnologyType.MapMakingIII, TownLeaderPosition.Economy, "Mapmaking IV", 5)
        },
        {
            TechnologyType.GemsI,
            new(TechnologyType.FreeTrade, TownLeaderPosition.Economy, "Gem Cutting I", 2)
        },
        {
            TechnologyType.GemsII,
            new(TechnologyType.GemsI, TownLeaderPosition.Economy, "Gem Cutting II", 3)
        },

        // ritual
        {
            TechnologyType.Ritual,
            new(null, TownLeaderPosition.Cosmology, "Ritual", 1)
        },
        {
            TechnologyType.ShamanismI,
            new(TechnologyType.Ritual, TownLeaderPosition.Cosmology, "Shamanism I", 2)
        },
        {
            TechnologyType.ShamanismII,
            new(TechnologyType.ShamanismI, TownLeaderPosition.Cosmology, "Shamanism II", 3)
        },
        {
            TechnologyType.TithesI,
            new(TechnologyType.Ritual, TownLeaderPosition.Cosmology, "Tithes I", 2)
        },
        {
            TechnologyType.TithesII,
            new(TechnologyType.TithesI, TownLeaderPosition.Cosmology, "Tithes II", 3)
        },
        {
            TechnologyType.TithesIII,
            new(TechnologyType.TithesII, TownLeaderPosition.Cosmology, "Tithes III", 4)
        },
        {
            TechnologyType.TithesIV,
            new(TechnologyType.TithesIII, TownLeaderPosition.Cosmology, "Tithes IV", 5)
        },

        // divination
        {
            TechnologyType.Divination,
            new(null, TownLeaderPosition.Cosmology, "Divination", 1)
        },
        {
            TechnologyType.SacrificeI,
            new(TechnologyType.Divination, TownLeaderPosition.Cosmology, "Sacrifice I", 2)
        },
        {
            TechnologyType.SacrificeII,
            new(TechnologyType.SacrificeI, TownLeaderPosition.Cosmology, "Sacrifice II", 3)
        },
        {
            TechnologyType.SacrificeIII,
            new(TechnologyType.SacrificeII, TownLeaderPosition.Cosmology, "Sacrifice III", 4)
        },
        {
            TechnologyType.AstrologyI,
            new(TechnologyType.Divination, TownLeaderPosition.Cosmology, "Astrology I", 2)
        },
        {
            TechnologyType.AstrologyII,
            new(TechnologyType.AstrologyI, TownLeaderPosition.Cosmology, "Astrology II", 3)
        },
        {
            TechnologyType.AstrologyIII,
            new(TechnologyType.AstrologyII, TownLeaderPosition.Cosmology, "Astrology III", 4)
        },
        {
            TechnologyType.AstrologyIV,
            new(TechnologyType.AstrologyIII, TownLeaderPosition.Cosmology, "Astrology IV", 5)
        },

        // militarization
        {
            TechnologyType.Militarization,
            new(null, TownLeaderPosition.Defense, "Militarization", 1)
        },
        {
            TechnologyType.FoundryI,
            new(TechnologyType.Militarization, TownLeaderPosition.Defense, "Foundry I", 2)
        },
        {
            TechnologyType.FoundryII,
            new(TechnologyType.FoundryI, TownLeaderPosition.Defense, "Foundry II", 3)
        },
        {
            TechnologyType.MilitiaI,
            new(TechnologyType.Militarization, TownLeaderPosition.Defense, "Militia I", 2)
        },
        {
            TechnologyType.MilitiaII,
            new(TechnologyType.MilitiaI, TownLeaderPosition.Defense, "Militia II", 3)
        },

        // expansion
        {
            TechnologyType.Expansion,
            new(null, TownLeaderPosition.Defense, "Expansion", 1)
        },
        {
            TechnologyType.TrackingI,
            new(TechnologyType.Expansion, TownLeaderPosition.Defense, "Tracking I", 2)
        },
        {
            TechnologyType.TrackingII,
            new(TechnologyType.TrackingI, TownLeaderPosition.Defense, "Tracking II", 3)
        },
        {
            TechnologyType.TrackingIII,
            new(TechnologyType.TrackingII, TownLeaderPosition.Defense, "Tracking III", 4)
        },

        // town beautification
        {
            TechnologyType.TownBeautification,
            new(null, TownLeaderPosition.Culture, "Town Beautification", 1)
        },
        {
            TechnologyType.PublicArtI,
            new(TechnologyType.TownBeautification, TownLeaderPosition.Culture, "Public Art I", 2)
        },
        {
            TechnologyType.PublicArtII,
            new(TechnologyType.PublicArtI, TownLeaderPosition.Culture, "Public Art II", 3)
        },
        {
            TechnologyType.PublicArtIII,
            new(TechnologyType.PublicArtII, TownLeaderPosition.Culture, "Public Art III", 4)
        },
        {
            TechnologyType.PublicArtIV,
            new(TechnologyType.PublicArtIII, TownLeaderPosition.Culture, "Public Art IV", 5)
        },
        {
            TechnologyType.TourismI,
            new(TechnologyType.TownBeautification, TownLeaderPosition.Culture, "Tourism I", 2)
        },
        {
            TechnologyType.TourismII,
            new(TechnologyType.TourismI, TownLeaderPosition.Culture, "Tourism II", 3)
        },
        {
            TechnologyType.TourismIII,
            new(TechnologyType.TourismII, TownLeaderPosition.Culture, "Tourism III", 4)
        }
    };

    public static List<TechnologyType> AvailableTechnologies(
        TownLeaderPosition position,
        List<TechnologyType> researchedTechnologies
    )
        => TechInfo
            .Where(kvp =>
                !researchedTechnologies.Contains(kvp.Key) &&
                kvp.Value.Category == position && (
                    kvp.Value.PreReq == null ||
                    researchedTechnologies.Contains(kvp.Value.PreReq.Value)
                )
            )
            .Select(kvp => kvp.Key)
            .ToList();

    public static async Task<bool> HasTechnology(Db db, Guid userId, TechnologyType tech, CancellationToken cToken)
        => await db.UserTechnologies.AnyAsync(ut => ut.UserId == userId && ut.Technology == tech, cToken);
}