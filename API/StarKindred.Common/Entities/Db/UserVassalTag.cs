using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class UserVassalTag: IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Title { get; set; } = null!;
    public string Color { get; set; } = null!;
    
    public List<Vassal>? Vassals { get; set; }

    public class Configuration : IEntityTypeConfiguration<UserVassalTag>
    {
        public void Configure(EntityTypeBuilder<UserVassalTag> builder)
        {
            builder.HasIndex(x => x.Title);
            builder.HasIndex(x => new { x.UserId, x.Title }).IsUnique();

            builder.Property(x => x.Title).HasMaxLength(20);

            builder.HasOne(x => x.User).WithMany(x => x.VassalTags).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(x => x.Vassals).WithMany(x => x.Tags);
        }
    }
}