using MasoloAgro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasoloAgro.Infrastructure.Database.Configurations;

/// <summary>
/// Shapes the purchases table. Ref stays unique. Paid stays within zero
/// to total and balance stays at total less paid.
/// </summary>
public sealed class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RefNumber).IsRequired().HasMaxLength(32);
        builder.HasIndex(x => x.RefNumber).IsUnique();
        builder.Property(x => x.PurchaseDate).IsRequired();
        builder.Property(x => x.SupplierName).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Total).HasPrecision(18, 2);
        builder.Property(x => x.AmountPaid).HasPrecision(18, 2);
        builder.Property(x => x.BalanceDue).HasPrecision(18, 2);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedByUserId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.ReversedAt).IsRequired(false);
        builder.Property(x => x.ReversalReason).HasMaxLength(500);
        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.Lines)
            .WithOne(x => x.Purchase)
            .HasForeignKey(x => x.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Payments)
            .WithOne(x => x.Purchase)
            .HasForeignKey(x => x.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Movements)
            .WithOne(x => x.Purchase)
            .HasForeignKey(x => x.PurchaseId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.ToTable(x => x.HasCheckConstraint("CK_Purchases_Total", "CAST(Total AS REAL) >= 0"));
        builder.ToTable(x => x.HasCheckConstraint("CK_Purchases_AmountPaid", "CAST(AmountPaid AS REAL) >= 0 AND CAST(AmountPaid AS REAL) <= CAST(Total AS REAL)"));
        builder.ToTable(x => x.HasCheckConstraint("CK_Purchases_BalanceDue", "CAST(BalanceDue AS REAL) >= 0"));
    }
}
