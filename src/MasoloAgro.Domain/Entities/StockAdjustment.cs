namespace MasoloAgro.Domain.Entities;

/// <summary>
/// Manual stock fix with a reason. Positive change is gain and negative
/// change is loss, never zero. Each fix links to a ledger movement.
/// </summary>
public sealed class StockAdjustment
{
    public Guid Id { get; set; }

    public Guid CommodityId { get; set; }

    public Commodity? Commodity { get; set; }

    public decimal QuantityChangeKg { get; set; }

    public string Reason { get; set; } = string.Empty;

    public Guid CreatedByUserId { get; set; }

    public User? CreatedBy { get; set; }

    public DateTime AdjustmentDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();
}
