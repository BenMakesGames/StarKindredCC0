using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class User: IEntity
{
    public Guid Id { get; set; }
    
    public UserAlliance? UserAlliance { get; set; }

    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastMissionCompletedOn { get; set; }
    public DateTimeOffset LastAttackedGiant { get; set; } = DateTimeOffset.UtcNow.AddDays(-1);
    public DateTimeOffset LastUsedRallyingStandard { get; set; } = DateTimeOffset.UtcNow.AddDays(-1);

    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTimeOffset? EmailVerifiedOn { get; set; }
    public string Passphrase { get; set; } = null!;

    public string Avatar { get; set; } = "black-and-white/rude-dragon";
    public HSL Color { get; set; } = new() { Hue = 210, Saturation = 75, Luminosity = 53 };

    public int Level { get; set; }

    public List<Resource>? Resources { get; set; }
    public List<Building>? Buildings { get; set; }
    public List<Mission>? Missions { get; set; }
    public List<UserVassalTag>? VassalTags { get; set; }
    public List<UserSubscription>? Subscriptions { get; set; }

    public class Configuration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.LastMissionCompletedOn);

            builder.Property(x => x.Name).HasMaxLength(20);
            builder.Property(x => x.Email).HasMaxLength(100);
            builder.Property(x => x.Passphrase).HasMaxLength(256);

            builder.OwnsOne(x => x.Color);
        }
    }
}
