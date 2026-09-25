namespace MasoloAgro.Domain.Entities;

/// <summary>
/// Money in against one parent only. A payment links to a sale or to a
/// purchase, exactly one of the two. Rows stay fixed, a fix adds a new row.
/// </summary>
public sealed class Payment
{
    public Guid Id { get; set; }

    public Guid? SaleId { get; set; }

    public Sale? Sale { get; set; }

    public Guid? PurchaseId { get; set; }

    public Purchase? Purchase { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public Guid ReceivedByUserId { get; set; }

    public User? ReceivedBy { get; set; }

    public string? Method { get; set; }

    public string? Notes { get; set; }
}
