using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class TownDecoration: IEntity
{
    public Guid Id { get; set; }
    
    public Guid TownId { get; set; }
    public Town? Town { get; set; }

    public DecorationType Type { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public int Scale { get; set; }
    public bool FlipX { get; set; }

    public class Configuration : IEntityTypeConfiguration<TownDecoration>
    {
        public void Configure(EntityTypeBuilder<TownDecoration> builder)
        {
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(40);
        }
    }
}