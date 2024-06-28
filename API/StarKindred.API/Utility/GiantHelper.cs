using System.Text;
using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.API.Entities;

namespace StarKindred.API.Utility;

public static class GiantHelper
{
    public static Giant CreateGiant(Random rng, int level, DateTimeOffset? lastGiantExpiredOn)
    {
        var element = rng.NextEnumValue<Element>();
        
        var startOn = DateTimeOffset.UtcNow.AddHours(10).Date;
        
        if(lastGiantExpiredOn.HasValue && lastGiantExpiredOn.Value.AddDays(1) > startOn)
            startOn = lastGiantExpiredOn.Value.AddDays(1).Date;
        
        var expiresOn = startOn.AddDays(2);
        
        return new Giant()
        {
            Element = element,
            Health = GiantHealth(level),
            StartsOn = startOn,
            ExpiresOn = expiresOn,
        };
    }
    
    public static int GiantHealth(int level)
    {
        return level * 100;
    }

    public static GiantRewards GiantRewards(Element element, int level)
    {
        var resources = new List<ResourceQuantity>();
        var treasures = new List<TreasureQuantity>();
        
        var bonusResource = element switch
        {
            Element.Earth => ResourceType.Stone,
            Element.Fire => ResourceType.Wood,
            Element.Ice => ResourceType.Marble,
            Element.Lightning => ResourceType.Gold,
            Element.Metal => ResourceType.Iron,
            Element.Plant => ResourceType.Wheat,
            Element.Water => ResourceType.Wine,
            _ => throw new Exception("Giant element not supported. (This is a bug!)")
        };
        
        resources.Add(new ResourceQuantity(ResourceType.Quintessence, (level * 5) + 5));
        resources.Add(new ResourceQuantity(bonusResource, (level * 5) + 45));

        treasures.Add(new(TreasureType.GoldChest, 1));
        
        if(level >= 2) // 2-4 => 1, 5-7 => 2, 8-12 => 3, ...
            treasures.Add(new(TreasureType.BoxOfOres, (int)Math.Sqrt(level * 2) - 1));

        if (level >= 3)
        {
            if(level >= 9)
                treasures.Add(new(TreasureType.BigBasicChest, level / 9));

            treasures.Add(new(TreasureType.BasicChest, level / 3 - (level / 9) * 2));
        }

        if(level >= 5)
            treasures.Add(new(TreasureType.RubyChest, level / 5));

        // 7-16 => 1, 17-30 => 2, 31-48 = >3, 49-70 => 4, ...
        if (level >= 7)
        {
            // ReSharper disable once PossibleLossOfFraction
            treasures.Add(new(TreasureType.WeaponChest, (int) Math.Sqrt((level + 1) / 2) - 1));
        }

        if(level >= 11)
            treasures.Add(new(TreasureType.TwilightChest, level / 11));

        return new(resources, treasures);
    }
}

public sealed record GiantRewards(List<ResourceQuantity> Resources, List<TreasureQuantity> Treasures)
{
    public string GetDescription()
    {
        var parts = new List<string>();
        
        parts.AddRange(Treasures.Select(t => t.Quantity == 1
            ? t.Type.ToNameWithArticle()
            : $"{t.Quantity} {t.Type.ToNamePlural()}"
        ));
        parts.AddRange(Resources.Select(r => $"{r.Quantity} {r.Type}"));

        return parts.ToNiceString();
    }

    public string GetMarkdownList()
    {
        var parts = new StringBuilder();

        foreach (var t in Treasures)
        {
            if (t.Quantity == 1)
                parts.Append($"* {t.Type.ToNameWithArticle()}\n");
            else
                parts.Append($"* {t.Quantity} {t.Type.ToNamePlural()}\n");
        }

        foreach(var r in Resources)
            parts.Append($"* {r.Quantity} {r.Type}\n");

        return parts.ToString();
    }
}