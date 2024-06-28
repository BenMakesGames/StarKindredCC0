using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class Adventure: IEntity
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public int ReleaseNumber { get; set; }
    public int ReleaseYear { get; set; }
    public int ReleaseMonth { get; set; }
    public bool IsDark { get; set; }

    public List<AdventureStep>? AdventureSteps { get; set; }

    public class Configuration : IEntityTypeConfiguration<Adventure>
    {
        public void Configure(EntityTypeBuilder<Adventure> builder)
        {
            builder.HasIndex(x => x.Title).IsUnique();
            builder.HasIndex(x => x.ReleaseNumber).IsUnique();
            builder.HasIndex(x => new { x.ReleaseYear, x.ReleaseMonth }).IsUnique();
        }
    }
}