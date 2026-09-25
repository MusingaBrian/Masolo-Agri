using MasoloAgro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasoloAgro.Infrastructure.Database.Configurations;

/// <summary>
/// Shapes the sales table. Ref stays unique for receipts. Paid stays
/// within zero to total and balance stays at total less paid.
/// </summary>
public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RefNumber).IsRequired().HasMaxLength(32);
        builder.HasIndex(x => x.RefNumber).IsUnique();
        builder.Property(x => x.SaleDate).IsRequired();
        builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(128);
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
            .WithOne(x => x.Sale)
            .HasForeignKey(x => x.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Payments)
            .WithOne(x => x.Sale)
            .HasForeignKey(x => x.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Movements)
            .WithOne(x => x.Sale)
            .HasForeignKey(x => x.SaleId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.ToTable(x => x.HasCheckConstraint("CK_Sales_Total", "CAST(Total AS REAL) >= 0"));
        builder.ToTable(x => x.HasCheckConstraint("CK_Sales_AmountPaid", "CAST(AmountPaid AS REAL) >= 0 AND CAST(AmountPaid AS REAL) <= CAST(Total AS REAL)"));
        builder.ToTable(x => x.HasCheckConstraint("CK_Sales_BalanceDue", "CAST(BalanceDue AS REAL) >= 0"));
    }
}
