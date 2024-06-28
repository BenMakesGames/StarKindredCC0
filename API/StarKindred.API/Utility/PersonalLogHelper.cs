using StarKindred.Common.Entities;
using StarKindred.Common.Entities.Db;
using StarKindred.Common.Services;

namespace StarKindred.API.Utility;

public static class PersonalLogHelper
{
    public static void Create(Db db, Guid userId, string message, IList<PersonalLogActivityType> tags)
    {
        db.PersonalLogs.Add(new()
        {
            UserId = userId,
            Message = message,
            PersonalLogTags = tags.Select(t => new PersonalLogTag() { Tag = t }).ToList()
        });
    }
    
    public static PersonalLogActivityType TagFromMissionOutcome(MissionOutcome outcome) => outcome switch
    {
        MissionOutcome.Bad => PersonalLogActivityType.Failure,
        MissionOutcome.Good => PersonalLogActivityType.Success,
        MissionOutcome.Great => PersonalLogActivityType.GreatSuccess,
        _ => throw new ArgumentException("Unsupported MissionOutcome", nameof(outcome))
    };
    
    public static PersonalLogActivityType TagFromMissionType(MissionType type) => type switch
    {
        MissionType.Oracle => PersonalLogActivityType.Oracle,
        MissionType.Settlers => PersonalLogActivityType.Settlers,
        MissionType.RecruitTown => PersonalLogActivityType.Recruit,
        MissionType.TreasureHunt => PersonalLogActivityType.TreasureHunt,
        MissionType.WanderingMonster => PersonalLogActivityType.MonsterHunt,
        MissionType.Gather => PersonalLogActivityType.Gather,
        MissionType.CollectStone => PersonalLogActivityType.CollectStone,
        MissionType.MineGold => PersonalLogActivityType.MineGold,
        MissionType.Story => PersonalLogActivityType.Story,
        MissionType.BoatDate => PersonalLogActivityType.BoatTour,
        MissionType.HuntLevel0 or
            MissionType.HuntLevel10 or
            MissionType.HuntLevel20 or
            MissionType.HuntLevel50 or
            MissionType.HuntLevel80 or
            MissionType.HuntLevel120 or
            MissionType.HuntLevel200 or
            MissionType.HuntAutoScaling => PersonalLogActivityType.AnimalHunt,
        _ => throw new ArgumentException("Unsupported MissionType", nameof(type))
    };

}