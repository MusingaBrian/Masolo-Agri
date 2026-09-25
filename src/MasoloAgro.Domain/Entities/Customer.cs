namespace MasoloAgro.Domain.Entities;

/// <summary>
/// Buyer master. Deals may link here and also keep a name copy taken
/// at deal time so quick cash deals still pass with no master row.
/// </summary>
public sealed class Customer
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
