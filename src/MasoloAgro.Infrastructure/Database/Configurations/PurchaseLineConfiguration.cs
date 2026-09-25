using MasoloAgro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasoloAgro.Infrastructure.Database.Configurations;

/// <summary>
/// Shapes the purchase lines table. Lines fall with the parent purchase
/// and a used commodity blocks delete.
/// </summary>
public sealed class PurchaseLineConfiguration : IEntityTypeConfiguration<PurchaseLine>
{
    public void Configure(EntityTypeBuilder<PurchaseLine> builder)
    {
        builder.ToTable("PurchaseLines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PurchaseId).IsRequired();
        builder.Property(x => x.CommodityId).IsRequired();
        builder.Property(x => x.QuantityKg).HasPrecision(18, 3);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);
        builder.HasOne(x => x.Commodity)
            .WithMany(x => x.PurchaseLines)
            .HasForeignKey(x => x.CommodityId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.ToTable(x => x.HasCheckConstraint("CK_PurchaseLines_QuantityKg", "CAST(QuantityKg AS REAL) > 0"));
        builder.ToTable(x => x.HasCheckConstraint("CK_PurchaseLines_UnitPrice", "CAST(UnitPrice AS REAL) >= 0"));
        builder.ToTable(x => x.HasCheckConstraint("CK_PurchaseLines_LineTotal", "CAST(LineTotal AS REAL) >= 0"));
    }
}
