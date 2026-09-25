namespace MasoloAgro.Domain.Entities;

/// <summary>
/// One sale row. Quantity stays in kg and price stays fixed at deal time.
/// Line total is quantity times price at write time.
/// </summary>
public sealed class SaleLine
{
    public Guid Id { get; set; }

    public Guid SaleId { get; set; }

    public Sale? Sale { get; set; }

    public Guid CommodityId { get; set; }

    public Commodity? Commodity { get; set; }

    public decimal QuantityKg { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}
