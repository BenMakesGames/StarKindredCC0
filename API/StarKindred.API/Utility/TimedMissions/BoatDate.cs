using BenMakesGames.RandomHelpers;
using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Humanizer;
using StarKindred.API.Endpoints.TimedMissions;

namespace StarKindred.API.Utility.TimedMissions;

public static class BoatDate
{
    public static async Task<Complete.ResponseDto> Do(
        Db db, Random rng, List<Vassal> vassals, CancellationToken cToken
    )
    {
        var roll = rng.Next(20);
        var outcome = MissionOutcome.Good;
        var relationshipMinutes = 8 * 60;
        string outcomeText;

        if (roll == 0)
        {
            outcome = MissionOutcome.Great;
            relationshipMinutes = 30;
            outcomeText = $"{vassals.Humanize(v => v.Name)} had a great time; the fisherfolk arranged an amazing tour, featuring {CoolSight(rng)}!";
        }
        else if (roll == 19 && !vassals.Any(v => v.Sign == AstrologicalSign.DoubleTrident))
        {
            outcome = MissionOutcome.Bad;
            relationshipMinutes = relationshipMinutes * 3 / 2;
            outcomeText = $"{vassals.Humanize(v => v.Name)} were having a nice-enough time, until the boat sprung a leak, and the tour had to be called off :(";
        }
        else
        {
            outcomeText = $"{vassals.Humanize(v => v.Name)} had a nice time; a relaxing break from the troubles of the larger world.";
        }

        var relationships = await RelationshipHelper.GetRelationships(db, vassals, cToken);

        RelationshipHelper.AdvanceRelationships(relationships, relationshipMinutes, cToken);

        return new Complete.ResponseDto(
            outcome,
            true,
            outcomeText,
            new()
        );
    }

    private static string CoolSight(Random rng) => rng.Next(new[]
    {
        "a beautiful waterfall",
        "a small island inhabited by incredible-looking birds",
        "a singing mermaid",
    });
}