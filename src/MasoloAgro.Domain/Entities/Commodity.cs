namespace MasoloAgro.Domain.Entities;

/// <summary>
/// Trade item. Stock stays in kg in the store. Price here is current
/// only and old deal lines keep their own deal time price.
/// </summary>
public sealed class Commodity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal CurrentPrice { get; set; }

    public decimal StockKg { get; set; }

    public decimal ReorderLevelKg { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<SaleLine> SaleLines { get; set; } = new List<SaleLine>();

    public ICollection<PurchaseLine> PurchaseLines { get; set; } = new List<PurchaseLine>();

    public ICollection<StockMovement> Movements { get; set; } = new List<StockMovement>();

    public ICollection<StockAdjustment> Adjustments { get; set; } = new List<StockAdjustment>();
}
