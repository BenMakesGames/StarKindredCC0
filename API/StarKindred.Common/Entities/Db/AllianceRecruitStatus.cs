using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class AllianceRecruitStatus: IEntity
{
    public Guid Id { get; set; }

    public Guid AllianceId { get; set; }
    public Alliance? Alliance { get; set; }

    public bool InviteCodeActive { get; set; }
    public string InviteCode { get; set; } = null!;
    public DateTimeOffset InviteCodeGeneratedOn { get; set; } = DateTimeOffset.UtcNow;

    public bool OpenInvitationActive { get; set; }
    public int OpenInvitationMinLevel { get; set; }
    public int OpenInvitationMaxLevel { get; set; } = 330;

    public class Configuration : IEntityTypeConfiguration<AllianceRecruitStatus>
    {
        public void Configure(EntityTypeBuilder<AllianceRecruitStatus> builder)
        {
            builder.HasOne(x => x.Alliance).WithOne(x => x.AllianceRecruitStatus).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
