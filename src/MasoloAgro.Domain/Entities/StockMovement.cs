using MasoloAgro.Domain.Enums;

namespace MasoloAgro.Domain.Entities;

/// <summary>
/// Audit row for every stock change. Rows never change in place, a fix
/// adds a new row. At most one source link is set per row.
/// </summary>
public sealed class StockMovement
{
    public Guid Id { get; set; }

    public Guid CommodityId { get; set; }

    public Commodity? Commodity { get; set; }

    public StockMovementType MovementType { get; set; } = StockMovementType.In;

    public decimal QuantityKg { get; set; }

    public DateTime MovementDate { get; set; }

    public Guid CreatedByUserId { get; set; }

    public User? CreatedBy { get; set; }

    public string? Reason { get; set; }

    public Guid? SaleId { get; set; }

    public Sale? Sale { get; set; }

    public Guid? PurchaseId { get; set; }

    public Purchase? Purchase { get; set; }

    public Guid? StockAdjustmentId { get; set; }

    public StockAdjustment? StockAdjustment { get; set; }
}
