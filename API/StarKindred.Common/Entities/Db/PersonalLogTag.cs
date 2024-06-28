using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class PersonalLogTag: IEntity
{
    public Guid Id { get; set; }

    public Guid PersonalLogId { get; set; }
    public PersonalLog? PersonalLog { get; set; }

    public PersonalLogActivityType Tag { get; set; }

    public class Configuration : IEntityTypeConfiguration<PersonalLogTag>
    {
        public void Configure(EntityTypeBuilder<PersonalLogTag> builder)
        {
            builder.Property(x => x.Tag).HasConversion<string>().HasMaxLength(20);
        }
    }
}