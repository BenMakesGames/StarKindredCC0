using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class Giant: IEntity
{
    public Guid Id { get; set; }
    
    public Guid AllianceId { get; set; }
    public Alliance? Alliance { get; set; }
    
    public DateTimeOffset StartsOn { get; set; }
    public DateTimeOffset ExpiresOn { get; set; }
    
    public Element Element { get; set; }
    
    public int Health { get; set; }
    public int Damage { get; set; }

    public class Configuration : IEntityTypeConfiguration<Giant>
    {
        public void Configure(EntityTypeBuilder<Giant> builder)
        {
            builder.Property(x => x.Element).HasConversion<string>().HasMaxLength(20);
        }
    }
}