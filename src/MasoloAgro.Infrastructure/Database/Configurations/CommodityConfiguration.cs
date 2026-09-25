using MasoloAgro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasoloAgro.Infrastructure.Database.Configurations;

/// <summary>
/// Shapes the commodities table. Money keeps two places and stock keeps
/// three places in kg. Name stays unique.
/// </summary>
public sealed class CommodityConfiguration : IEntityTypeConfiguration<Commodity>
{
    public void Configure(EntityTypeBuilder<Commodity> builder)
    {
        builder.ToTable("Commodities");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.CurrentPrice).HasPrecision(18, 2);
        builder.Property(x => x.StockKg).HasPrecision(18, 3);
        builder.Property(x => x.ReorderLevelKg).HasPrecision(18, 3);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.ToTable(x => x.HasCheckConstraint("CK_Commodities_CurrentPrice", "CAST(CurrentPrice AS REAL) >= 0"));
        builder.ToTable(x => x.HasCheckConstraint("CK_Commodities_StockKg", "CAST(StockKg AS REAL) >= 0"));
        builder.ToTable(x => x.HasCheckConstraint("CK_Commodities_ReorderLevelKg", "CAST(ReorderLevelKg AS REAL) >= 0"));
    }
}
