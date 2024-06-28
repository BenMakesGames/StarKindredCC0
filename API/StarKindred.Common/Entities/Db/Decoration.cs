using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class Decoration: IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public DecorationType Type { get; set; }
    public int Quantity { get; set; }
    
    public class Configuration : IEntityTypeConfiguration<Decoration>
    {
        public void Configure(EntityTypeBuilder<Decoration> builder)
        {
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(40);
            builder.HasIndex(x => x.Type);
        }
    }
}