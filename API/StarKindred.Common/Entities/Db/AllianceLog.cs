using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class AllianceLog: IEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    
    public Guid AllianceId { get; set; }
    public Alliance? Alliance { get; set; }
    
    public string Message { get; set; } = null!;
    public AllianceLogActivityType ActivityType { get; set; }

    public class Configuration : IEntityTypeConfiguration<AllianceLog>
    {
        public void Configure(EntityTypeBuilder<AllianceLog> builder)
        {
            builder.HasIndex(x => x.CreatedOn);
            
            builder.Property(x => x.ActivityType).HasConversion<string>().HasMaxLength(20);
        }
    }
}