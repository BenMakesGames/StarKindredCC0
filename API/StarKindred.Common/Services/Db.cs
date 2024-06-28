using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StarKindred.Common.Entities.Db;

namespace StarKindred.Common.Services;

public sealed class Db: DbContext
{
    public DbSet<Adventure> Adventures => Set<Adventure>();
    public DbSet<AdventureStep> AdventureSteps => Set<AdventureStep>();
    public DbSet<Alliance> Alliances => Set<Alliance>();
    public DbSet<AllianceLog> AllianceLogs => Set<AllianceLog>();
    public DbSet<AllianceRank> AllianceRanks => Set<AllianceRank>();
    public DbSet<AllianceRecruitStatus> AllianceRecruitStatuses => Set<AllianceRecruitStatus>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AnnouncementView> AnnouncementViews => Set<AnnouncementView>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Decoration> Decorations => Set<Decoration>();
    public DbSet<Goodie> Goodies => Set<Goodie>();
    public DbSet<Giant> Giants => Set<Giant>();
    public DbSet<GiantContribution> GiantContributions => Set<GiantContribution>();
    public DbSet<MagicLogin> MagicLogins => Set<MagicLogin>();
    public DbSet<Mission> Missions => Set<Mission>();
    public DbSet<PersonalLog> PersonalLogs => Set<PersonalLog>();
    public DbSet<PersonalLogTag> PersonalLogTags => Set<PersonalLogTag>();
    public DbSet<Relationship> Relationships => Set<Relationship>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<StatusEffect> StatusEffects => Set<StatusEffect>();
    public DbSet<TimedMission> TimedMissions => Set<TimedMission>();
    public DbSet<Town> Towns => Set<Town>();
    public DbSet<TownDecoration> TownDecorations => Set<TownDecoration>();
    public DbSet<TownLeader> TownLeaders => Set<TownLeader>();
    public DbSet<Treasure> Treasures => Set<Treasure>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserAdventure> UserAdventures => Set<UserAdventure>();
    public DbSet<UserAdventureStepCompleted> UserAdventureStepCompleted => Set<UserAdventureStepCompleted>();
    public DbSet<UserAdventureStepInProgress> UserAdventureStepInProgress => Set<UserAdventureStepInProgress>();
    public DbSet<UserAlliance> UserAlliances => Set<UserAlliance>();
    public DbSet<UserResearch> UserResearches => Set<UserResearch>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<UserTechnology> UserTechnologies => Set<UserTechnology>();
    public DbSet<UserUnlockedAvatar> UserUnlockedAvatars => Set<UserUnlockedAvatar>();
    public DbSet<UserVassalTag> UserVassalTags => Set<UserVassalTag>();
    public DbSet<Vassal> Vassals => Set<Vassal>();
    public DbSet<Weapon> Weapons => Set<Weapon>();

    public Db(DbContextOptions<Db> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}