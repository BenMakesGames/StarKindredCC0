using BenMakesGames.RandomHelpers;
using StarKindred.API.Entities;
using StarKindred.API.Services;
using StarKindred.Common.Entities;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility.Buildings.Powers;

public static class PasturePowers
{
    public static async Task<(string, ResourceQuantity?, bool)> DoHomesteading(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        Random rng,
        CancellationToken cToken
    )
    {
        var decoration = rng.Next(new List<DecorationType>()
        {
            DecorationType.OneAnimal,
            DecorationType.TwoAnimals,
            DecorationType.ThreeAnimals,
            DecorationType.FenceNorthSouth,
            DecorationType.FenceEastWest,
        });

        await DecorationHelper.CollectDecoration(db, session.UserId, decoration, 1, cToken);

        var message = $"The townsfolk offer you {decoration.ToNameWithArticle()}.";

        var upgraded = await TownHelpers.MakeDecorable(db, session.UserId, cToken);

        if (upgraded)
            message += "\n\n(You can now place Decorations in your Town!)";

        return (message, null, upgraded);
    }

    public static async Task<(string, ResourceQuantity?, bool)> DoSacrifice(
        Db db,
        ICurrentUser.CurrentSessionDto session,
        List<TechnologyType> researchedTechnologies,
        CancellationToken cToken
    )
    {
        var applicableTechnologies = new[]
        {
            TechnologyType.SacrificeII,
            TechnologyType.SacrificeIII
        };

        var boostingTechs = researchedTechnologies.Count(t => applicableTechnologies.Contains(t));

        var quint = new ResourceQuantity(ResourceType.Quintessence, 50 + boostingTechs * 25);

        await ResourceHelper.CollectResources(db, session.UserId, new() { quint }, cToken);

        return ($"A sacrifice to the gods yields {quint.Quantity} Quintessence.", quint, false);
    }

    public static (string, ResourceQuantity?, bool) DoGetScythe(
        Db db, Random rng, ICurrentUser.CurrentSessionDto session, List<TechnologyType> technologies
    )
    {
        var weapon = WeaponHelper.CollectWeapon(db, rng, session.UserId, WeaponBonus.WeaponsGetWheat);

        weapon.Level += technologies.Count(t => t is TechnologyType.MilitiaII or TechnologyType.FoundryII);

        return ($"The farmers give you one of their scythes: a Level-{weapon.Level} {weapon.Name}.", null, false);
    }
}