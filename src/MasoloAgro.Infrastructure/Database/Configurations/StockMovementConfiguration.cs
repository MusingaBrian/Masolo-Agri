using MasoloAgro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasoloAgro.Infrastructure.Database.Configurations;

/// <summary>
/// Shapes the ledger table. Quantity stays positive with direction in the
/// type. At most one source link is set per row so each move traces clean.
/// </summary>
public sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CommodityId).IsRequired();
        builder.Property(x => x.MovementType).IsRequired();
        builder.Property(x => x.QuantityKg).HasPrecision(18, 3);
        builder.Property(x => x.MovementDate).IsRequired();
        builder.Property(x => x.CreatedByUserId).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.HasOne(x => x.Commodity)
            .WithMany(x => x.Movements)
            .HasForeignKey(x => x.CommodityId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(x => x.CommodityId);
        builder.HasIndex(x => x.MovementDate);
        builder.ToTable(x => x.HasCheckConstraint("CK_StockMovements_QuantityKg", "CAST(QuantityKg AS REAL) > 0"));
        builder.ToTable(x => x.HasCheckConstraint(
            "CK_StockMovements_OneSource",
            "(CASE WHEN SaleId IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN PurchaseId IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN StockAdjustmentId IS NOT NULL THEN 1 ELSE 0 END) <= 1"));
    }
}
