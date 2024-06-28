using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;
using StarKindred.API.Services;
using StarKindred.API.Utility;

namespace StarKindred.API.Endpoints.Vassals;

[ApiController]
public sealed class Retire
{
    [HttpPost("/vassals/{vassalId:guid}/retire")]
    public async Task<ApiResponse> _(
        Guid vassalId,
        [FromServices] Db db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cToken
    )
    {
        var session = await currentUser.GetSessionOrThrow(cToken);
        var user = await db.Users.FirstAsync(u => u.Id == session.UserId, cToken);

        var vassal = await db.Vassals
            .Include(v => v.Leader)
            .FirstOrDefaultAsync(v => v.Id == vassalId && v.UserId == session.UserId, cToken)
            ?? throw new NotFoundException("That Vassal does not exist.");

        if (vassal.Favorite)
            throw new UnprocessableEntity("You cannot retired a Favorite Vassal.");

        if(vassal.IsOnAMission)
            throw new UnprocessableEntity("You cannot retire a Vassal while they're on a mission.");

        if(vassal.Leader != null)
            throw new UnprocessableEntity("You cannot retire a Vassal while they hold a leadership position.");

        if(vassal.RetirementPoints < 10)
            throw new UnprocessableEntity("This Vassal is not ready to Retire.");

        if ((await db.Vassals.CountAsync(v => v.UserId == session.UserId, cToken)) == 1)
            throw new UnprocessableEntity("You can't Retire your _only_ Vassal! Recruit some more Vassals, first.");

        var markdownLootList = await CollectRetirementRewards(db, session.UserId, vassal, cToken);

        PersonalLogHelper.Create(db, session.UserId, $"You retired **{vassal.Name}**. They sent you:{markdownLootList}", new[]
        {
            PersonalLogActivityType.Vassal,
            PersonalLogActivityType.RetiredVassal
        });

        db.Vassals.Remove(vassal);

        await db.SaveChangesAsync(cToken);

        await UserHelper.ComputeLevel(db, user, cToken);

        await db.SaveChangesAsync(cToken);

        return new()
        {
            Messages = new()
            {
                ApiMessage.Info($"{vassal.Name} went on their Retirement Journey. They sent you:{markdownLootList}"),
            }
        };
    }

    public static async Task<string> CollectRetirementRewards(Db db, Guid userId, Vassal vassal, CancellationToken cToken)
    {
        var rewards = BaseRewards(vassal.Level);

        rewards.AddRange(vassal.Nature switch
        {
            Nature.Cavalier => CollectCavalierRewards(vassal.Level),
            Nature.Competitor => CollectCompetitorRewards(vassal.Level),
            Nature.Defender => CollectDefenderRewards(vassal.Level),
            Nature.Evangelist => CollectEvangelistRewards(vassal.Level),
            Nature.Explorer => CollectExplorerRewards(vassal.Level),
            Nature.Loner => CollectLonerRewards(vassal.Level),
            Nature.Monger => CollectMongerRewards(vassal.Level),
            Nature.Perfectionist => CollectPerfectionistRewards(vassal.Level),
            Nature.ThrillSeeker => CollectThrillSeekerRewards(vassal.Level),
            Nature.Visionary => CollectVisionaryRewards(vassal.Level),
            _ => throw new ArgumentException($"Retirement rewards for {vassal.Nature} Vassals have not been implemented!")
        });

        foreach(var reward in rewards)
        {
            await TreasureHelper.CollectTreasure(db, userId, reward.Item1, reward.Item2, cToken);
        }
        
        var strings = rewards.Select(r => $"{r.Item2}× {r.Item1.ToName()}");

        return "\n* " + string.Join("\n* ", strings);
    }

    public static List<(TreasureType, int)> BaseRewards(int level)
    {
        var rewards = new List<(TreasureType, int)>();

        if (level >= 20)
            rewards.Add((TreasureType.BoxOfOres, level / 120 + 1));

        if (level >= 40)
            rewards.Add((TreasureType.BasicChest, level / 140 + 1));

        if (level >= 60)
            rewards.Add((TreasureType.RubyChest, level / 160 + 1));

        if (level >= 80)
            rewards.Add((TreasureType.WeaponChest, level / 180 + 1));

        if (level >= 100)
            rewards.Add((TreasureType.TwilightChest, level / 200 + 1));

        return rewards;
    }

    private static List<(TreasureType, int)> CollectCavalierRewards(int level)
    {
        return new List<(TreasureType, int)>()
        {
            (TreasureType.RallyingStandard, level / 75 + 1),
        };
    }

    private static List<(TreasureType, int)> CollectCompetitorRewards(int level)
    {
        return new List<(TreasureType, int)>()
        {
            (TreasureType.WrappedSword, level / 75 + 1),
        };
    }

    private static List<(TreasureType, int)> CollectDefenderRewards(int level)
    {
        return new List<(TreasureType, int)>()
        {
            (TreasureType.WrappedSword, level / 75 + 1),
        };
    }

    private static List<(TreasureType, int)> CollectEvangelistRewards(int level)
    {
        return new List<(TreasureType, int)>()
        {
            (TreasureType.Soma, level / 75 + 1)
        };
    }

    private static List<(TreasureType, int)> CollectExplorerRewards(int level)
    {
        return new List<(TreasureType, int)>()
        {
            (TreasureType.TreasureMap, level / 75 + 1)
        };
    }

    private static List<(TreasureType, int)> CollectLonerRewards(int level)
    {
        return new List<(TreasureType, int)>()
        {
            (TreasureType.Soma, level / 75 + 1)
        };
    }

    private static List<(TreasureType, int)> CollectMongerRewards(int level)
    {
        return new List<(TreasureType, int)>()
        {
            (TreasureType.GoldChest, level / 75 + 1),
        };
    }

    private static List<(TreasureType, int)> CollectPerfectionistRewards(int level)
    {
        return new List<(TreasureType, int)>()
        {
            (TreasureType.GoldChest, level / 75 + 1),
        };
    }

    private static List<(TreasureType, int)> CollectThrillSeekerRewards(int level)
    {
        return new List<(TreasureType, int)>()
        {
            (TreasureType.TreasureMap, level / 75 + 1)
        };
    }

    private static List<(TreasureType, int)> CollectVisionaryRewards(int level)
    {
        return new List<(TreasureType, int)>()
        {
            (TreasureType.Soma, level / 75 + 1)
        };
    }
}