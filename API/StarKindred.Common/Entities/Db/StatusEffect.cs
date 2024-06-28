using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class StatusEffect: IEntity
{
    public Guid Id { get; set; }

    public Guid VassalId { get; set; }
    public Vassal? Vassal { get; set; }

    public int Strength { get; set; }

    public StatusEffectType Type { get; set; }

    public class Configuration : IEntityTypeConfiguration<StatusEffect>
    {
        public void Configure(EntityTypeBuilder<StatusEffect> builder)
        {
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(40);

            builder.HasOne(x => x.Vassal).WithMany(v => v.StatusEffects).OnDelete(DeleteBehavior.Cascade);
        }
    }
}