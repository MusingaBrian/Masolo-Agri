using MasoloAgro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasoloAgro.Infrastructure.Database;

/// <summary>
/// EF Core context for the local offline SQLite database.
/// All twelve sets stay here. The UI never touches this type, all use
/// flows through app services over the typed bridge.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public DbSet<Commodity> Commodities => Set<Commodity>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<SaleLine> SaleLines => Set<SaleLine>();

    public DbSet<Purchase> Purchases => Set<Purchase>();

    public DbSet<PurchaseLine> PurchaseLines => Set<PurchaseLine>();

    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();

    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // SQLite keeps decimal values as text, so each numeric guard in the
        // configs below compares after CAST to REAL. Plain compare would
        // treat text as greater than any number and the guard would not fire.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
