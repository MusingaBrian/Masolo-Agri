using MasoloAgro.Domain.Enums;

namespace MasoloAgro.Domain.Entities;

/// <summary>
/// Sale header. Total is the sum of line totals at write time, paid is
/// the sum of linked payments at write time, balance is total less paid.
/// </summary>
public sealed class Sale
{
    public Guid Id { get; set; }

    public string RefNumber { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public Guid? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal BalanceDue { get; set; }

    public SaleStatus Status { get; set; } = SaleStatus.Posted;

    public Guid CreatedByUserId { get; set; }

    public User? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReversedAt { get; set; }

    public string? ReversalReason { get; set; }

    public ICollection<SaleLine> Lines { get; set; } = new List<SaleLine>();

    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
