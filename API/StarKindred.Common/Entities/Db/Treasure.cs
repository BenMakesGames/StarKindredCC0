using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class Treasure: IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public TreasureType Type { get; set; }
    public int Quantity { get; set; }

    public class Configuration : IEntityTypeConfiguration<Treasure>
    {
        public void Configure(EntityTypeBuilder<Treasure> builder)
        {
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(40);
        }
    }
}