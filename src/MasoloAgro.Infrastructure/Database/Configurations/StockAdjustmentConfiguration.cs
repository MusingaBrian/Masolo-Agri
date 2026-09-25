using MasoloAgro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasoloAgro.Infrastructure.Database.Configurations;

/// <summary>
/// Shapes the adjustments table. Change never stays zero and a reason is
/// always kept so each manual fix is clear.
/// </summary>
public sealed class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.ToTable("StockAdjustments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CommodityId).IsRequired();
        builder.Property(x => x.QuantityChangeKg).HasPrecision(18, 3);
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(500);
        builder.Property(x => x.CreatedByUserId).IsRequired();
        builder.Property(x => x.AdjustmentDate).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasOne(x => x.Commodity)
            .WithMany(x => x.Adjustments)
            .HasForeignKey(x => x.CommodityId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.Movements)
            .WithOne(x => x.StockAdjustment)
            .HasForeignKey(x => x.StockAdjustmentId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.ToTable(x => x.HasCheckConstraint("CK_StockAdjustments_Change", "CAST(QuantityChangeKg AS REAL) <> 0"));
    }
}
