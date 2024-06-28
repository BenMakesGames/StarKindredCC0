using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class Alliance: IEntity
{
    public const int MaxMemberCount = 20;

    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastActiveOn { get; set; }

    public List<UserAlliance>? Members { get; set; }
    public Giant? Giant { get; set; }

    public Guid LeaderId { get; set; }
    public User? Leader { get; set; }

    public int Level { get; set; } = 1;
    
    public List<AllianceLog>? Logs { get; set; }

    public List<AllianceRank>? AllianceRanks { get; set; }

    public AllianceRecruitStatus? AllianceRecruitStatus { get; set; }

    public class Configuration : IEntityTypeConfiguration<Alliance>
    {
        public void Configure(EntityTypeBuilder<Alliance> builder)
        {
            builder.HasIndex(x => x.LastActiveOn);
            builder.HasIndex(x => x.CreatedOn);
        }
    }
}