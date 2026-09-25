using MasoloAgro.Domain.Enums;

namespace MasoloAgro.Domain.Entities;

/// <summary>
/// Purchase header. Total is the sum of line totals at write time, paid
/// is the sum of linked payments at write time, balance is total less paid.
/// </summary>
public sealed class Purchase
{
    public Guid Id { get; set; }

    public string RefNumber { get; set; } = string.Empty;

    public DateTime PurchaseDate { get; set; }

    public Guid? SupplierId { get; set; }

    public Supplier? Supplier { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal BalanceDue { get; set; }

    public PurchaseStatus Status { get; set; } = PurchaseStatus.Posted;

    public Guid CreatedByUserId { get; set; }

    public User? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReversedAt { get; set; }

    public string? ReversalReason { get; set; }

    public ICollection<PurchaseLine> Lines { get; set; } = new List<PurchaseLine>();

    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
