using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class TimedMission
{
    public Guid Id { get; set; }
    public MissionType Type { get; set; }
    public int Location { get; set; }
    public string Description { get; set; } = null!;

    // for wandering monsters
    public Element? Element { get; set; }

    // monsters & treasure hunts    
    public TreasureType? Treasure { get; set; }
    public WeaponBonus? Weapon { get; set; }

    // monsters, treasure hunts, and settlers
    public int Level { get; set; }

    // settlers
    public Species? Species { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? StartedOn { get; set; }
    public DateTimeOffset? CompletesOn { get; set; }

    public List<Vassal>? Vassals { get; set; }

    public class Configuration : IEntityTypeConfiguration<TimedMission>
    {
        public void Configure(EntityTypeBuilder<TimedMission> builder)
        {
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Treasure).HasConversion<string>().HasMaxLength(40);
            builder.Property(x => x.Weapon).HasConversion<string>().HasMaxLength(40);
            builder.Property(x => x.Element).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Species).HasConversion<string>().HasMaxLength(20);
        }
    }
}
