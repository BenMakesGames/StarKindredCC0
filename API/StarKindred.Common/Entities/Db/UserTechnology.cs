using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class UserTechnology: IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public TechnologyType Technology { get; set; }

    public DateTimeOffset CompletedOn { get; set; } = DateTimeOffset.UtcNow;

    public class Configuration : IEntityTypeConfiguration<UserTechnology>
    {
        public void Configure(EntityTypeBuilder<UserTechnology> builder)
        {
            builder.Property(x => x.Technology).HasConversion<string>().HasMaxLength(40);
            builder.HasIndex(x => new { x.UserId, x.Technology }).IsUnique();
        }
    }
}