using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class Vassal: IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid? MissionId { get; set; }
    public Mission? Mission { get; set; }

    public Guid? TimedMissionId { get; set; }
    public TimedMission? TimedMission { get; set; }

    public Guid? UserAdventureStepInProgressId { get; set; }
    public UserAdventureStepInProgress? UserAdventureStepInProgress { get; set; }

    public bool IsOnAMission => MissionId != null || TimedMissionId != null || UserAdventureStepInProgressId != null;

    public Guid? WeaponId { get; set; }
    public Weapon? Weapon { get; set; }

    public string Name { get; set; } = null!;
    public string Portrait { get; set; } = null!;

    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;

    public int Level { get; set; }
    public int Willpower { get; set; }
    public int RetirementPoints { get; set; }
    public bool Favorite { get; set; }

    public Element Element { get; set; }
    public Species Species { get; set; }
    public AstrologicalSign Sign { get; set; }
    public Nature Nature { get; set; }
    
    public List<StatusEffect>? StatusEffects { get; set; }
    public List<UserVassalTag>? Tags { get; set; }
    public List<Relationship>? Relationships { get; set; }
    public TownLeader? Leader { get; set; }

    public class Configuration : IEntityTypeConfiguration<Vassal>
    {
        public void Configure(EntityTypeBuilder<Vassal> builder)
        {
            builder.HasIndex(x => x.Name);
            builder.HasIndex(x => x.Level);
            builder.HasIndex(x => x.Favorite);

            builder.Property(x => x.Name).HasMaxLength(30);
            builder.Property(x => x.Portrait).HasMaxLength(20);
            builder.Property(x => x.Element).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Nature).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Species).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Sign).HasConversion<string>().HasMaxLength(20);

            builder.HasMany(x => x.Relationships).WithMany(x => x.Vassals);
        }
    }
}