using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class Mission: IEntity
{
    public Guid Id { get; set; }
    public MissionType Type { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;

    public List<Vassal>? Vassals { get; set; }

    public class Configuration : IEntityTypeConfiguration<Mission>
    {
        public void Configure(EntityTypeBuilder<Mission> builder)
        {
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        }
    }
}