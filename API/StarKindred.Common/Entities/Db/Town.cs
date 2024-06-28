using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class Town : IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Name { get; set; } = "Town";
    public int Level { get; set; }
    public bool CanDecorate { get; set; }

    public DateTimeOffset NextRumor { get; set; }
    public DateTimeOffset LastGoodie { get; set; } = DateTimeOffset.UtcNow;
    
    public List<TownDecoration>? Decorations { get; set; }

    public class Configuration : IEntityTypeConfiguration<Town>
    {
        public void Configure(EntityTypeBuilder<Town> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(20);
        }
    }
}
