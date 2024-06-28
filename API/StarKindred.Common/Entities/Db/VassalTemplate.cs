using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class VassalTemplate: IEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
    public string? Portrait { get; set; }
    public Element? Element { get; set; }
    public Species? Species { get; set; }
    public AstrologicalSign? Sign { get; set; }
    public Nature? Nature { get; set; }

    public class Configuration : IEntityTypeConfiguration<VassalTemplate>
    {
        public void Configure(EntityTypeBuilder<VassalTemplate> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(30);
            builder.Property(x => x.Portrait).HasMaxLength(20);
            builder.Property(x => x.Element).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Nature).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Species).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Sign).HasConversion<string>().HasMaxLength(20);
        }
    }
}