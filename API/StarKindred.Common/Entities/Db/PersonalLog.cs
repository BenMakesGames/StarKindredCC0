using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class PersonalLog: IEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Message { get; set; } = null!;
    
    public List<PersonalLogTag>? PersonalLogTags { get; set; }

    public class Configuration : IEntityTypeConfiguration<PersonalLog>
    {
        public void Configure(EntityTypeBuilder<PersonalLog> builder)
        {
            builder.HasIndex(x => x.CreatedOn);
        }
    }
}