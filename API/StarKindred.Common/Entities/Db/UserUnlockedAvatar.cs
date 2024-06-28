using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class UserUnlockedAvatar: IEntity
{
    public Guid Id { get; set; }

    public DateTimeOffset UnlockedOn { get; set; } = DateTimeOffset.UtcNow;

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Avatar { get; set; } = null!;

    public class Configuration : IEntityTypeConfiguration<UserUnlockedAvatar>
    {
        public void Configure(EntityTypeBuilder<UserUnlockedAvatar> builder)
        {
            builder.HasIndex(x => x.UnlockedOn);
            builder.Property(x => x.Avatar).HasMaxLength(40);
        }
    }
}