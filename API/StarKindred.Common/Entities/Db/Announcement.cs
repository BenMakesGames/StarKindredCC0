using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class Announcement: IEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public AnnouncementType Type { get; set; }
    public string Body { get; set; } = null!;

    public class Configuration : IEntityTypeConfiguration<Announcement>
    {
        public void Configure(EntityTypeBuilder<Announcement> builder)
        {
            builder.HasIndex(x => x.CreatedOn);
            
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        }
    }
}
