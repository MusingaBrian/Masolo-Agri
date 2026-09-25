using MasoloAgro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasoloAgro.Infrastructure.Database.Configurations;

/// <summary>
/// Shapes the payments table. Each row links to exactly one parent, a
/// sale or a purchase, and amount stays above zero.
/// </summary>
public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.PaymentDate).IsRequired();
        builder.Property(x => x.ReceivedByUserId).IsRequired();
        builder.Property(x => x.Method).HasMaxLength(64);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasOne(x => x.ReceivedBy)
            .WithMany()
            .HasForeignKey(x => x.ReceivedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.ToTable(x => x.HasCheckConstraint("CK_Payments_Amount", "CAST(Amount AS REAL) > 0"));
        builder.ToTable(x => x.HasCheckConstraint(
            "CK_Payments_OneParent",
            "(SaleId IS NOT NULL AND PurchaseId IS NULL) OR (SaleId IS NULL AND PurchaseId IS NOT NULL)"));
    }
}
