using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class UserAlliance: IEntity
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public Guid AllianceId { get; set; }
    public Alliance? Alliance { get; set; }

    public Guid? AllianceRankId { get; set; }
    public AllianceRank? AllianceRank { get; set; }

    public DateTimeOffset JoinedOn { get; set; } = DateTimeOffset.UtcNow;

    public class Configuration : IEntityTypeConfiguration<UserAlliance>
    {
        public void Configure(EntityTypeBuilder<UserAlliance> builder)
        {
            builder.HasOne(x => x.AllianceRank).WithMany(x => x.UserAlliances).OnDelete(DeleteBehavior.SetNull);
        }
    }
}