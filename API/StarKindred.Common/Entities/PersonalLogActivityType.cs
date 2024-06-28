namespace StarKindred.Common.Entities;

public enum PersonalLogActivityType
{
    AccountActivity,
    AccountCreated,
    LoggedIn,
    UpdatedAvatarImage,
    UpdatedAvatarColor,
    UpdatedEmail,
    UpdatedPassphrase,

    CompleteMission,
    Failure,
    Success,
    GreatSuccess,

    Oracle,
    Settlers,
    AnimalHunt,
    Recruit,
    TreasureHunt,
    MonsterHunt,
    Gather,
    Story,
    BoatTour,
    CollectStone,
    MineGold,

    Building,
    BuildBuilding,
    SpecializedBuilding,
    LeveledUpBuilding,
    RebuildBuilding,

    Vassal,
    RenamedVassal,
    RetiredVassal,
    DismissedVassal,
    LeveledUpVassal,
    SpentWillpower,

    Leader,
    StartedProject,
    CancelledProject,
    CompletedProject,
}