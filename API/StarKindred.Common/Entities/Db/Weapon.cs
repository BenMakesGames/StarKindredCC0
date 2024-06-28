using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StarKindred.Common.Entities.Db;

public class Weapon: IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public int Level { get; set; } = 1;

    public int MaxDurability { get; set; }
    public int Durability { get; set; }

    public WeaponBonus PrimaryBonus { get; set; }
    public WeaponBonus SecondaryBonus { get; set; }

    public Vassal? Vassal { get; set; }

    public class Configuration : IEntityTypeConfiguration<Weapon>
    {
        public void Configure(EntityTypeBuilder<Weapon> builder)
        {
            builder.HasIndex(x => x.Level);
            
            builder.Property(x => x.PrimaryBonus).HasConversion<string>().HasMaxLength(40);
            builder.Property(x => x.SecondaryBonus).HasConversion<string>().HasMaxLength(40);
            
            builder.Property(x => x.Name).HasMaxLength(20);
            builder.Property(x => x.Image).HasMaxLength(40);
        }
    }
}