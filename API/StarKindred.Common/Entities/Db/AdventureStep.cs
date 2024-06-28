using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class AdventureStep: IEntity
{
    public Guid Id { get; set; }

    public Guid AdventureId { get; set; }
    public Adventure? Adventure { get; set; }

    public string Title { get; set; } = null!;
    public MissionType Type { get; set; }

    public int Step { get; set; }
    public int? PreviousStep { get; set; }

    public float X { get; set; }
    public float Y { get; set; }

    public int MinVassals { get; set; }
    public int MaxVassals { get; set; }

    public Element? RequiredElement { get; set; }

    public int DurationInMinutes { get; set; }

    public string? Narrative { get; set; }
    public TreasureType? Treasure { get; set; }
    public DecorationType? Decoration { get; set; }
    public string? UnlockableAvatar { get; set; }
    public Guid? RecruitId { get; set; }
    public VassalTemplate? Recruit { get; set; }

    public PinSide? PinOverride { get; set; }

    public List<UserAdventureStepCompleted>? UserAdventureStepsCompleted { get; set; }

    public class Configuration : IEntityTypeConfiguration<AdventureStep>
    {
        public void Configure(EntityTypeBuilder<AdventureStep> builder)
        {
            builder.HasIndex(x => new { x.AdventureId, x.Step }).IsUnique();

            builder.Property(x => x.Title).HasMaxLength(30);

            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.RequiredElement).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Treasure).HasConversion<string>().HasMaxLength(40);
            builder.Property(x => x.Decoration).HasConversion<string>().HasMaxLength(40);
            builder.Property(x => x.UnlockableAvatar).HasMaxLength(40);

            builder.Property(x => x.PinOverride).HasConversion<string>().HasMaxLength(10);
        }
    }
}