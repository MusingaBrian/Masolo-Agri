using MasoloAgro.Domain.Entities;
using MasoloAgro.Domain.Enums;
using MasoloAgro.Infrastructure.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MasoloAgro.Tests;

/// <summary>
/// Model proof for money plus kg plus deal time prices plus totals math.
/// Covers AC-2 plus AC-3 plus AC-4 from spec 0001.
/// </summary>
[TestClass]
public sealed class SchemaModelTests
{
    private static AppDbContext CreateDb(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        var db = new AppDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    private static User NewUser(string username, UserRole role = UserRole.Owner)
    {
        var now = DateTime.UtcNow;
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = "hash",
            Role = role,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private static Commodity NewCommodity(string name, decimal price, decimal stockKg = 0)
    {
        var now = DateTime.UtcNow;
        return new Commodity
        {
            Id = Guid.NewGuid(),
            Name = name,
            CurrentPrice = price,
            StockKg = stockKg,
            ReorderLevelKg = 10,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    [TestMethod]
    public async Task Money_StaysDecimal_WithWholeShillings()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("moneyowner");
        var maize = NewCommodity("MaizeMoney", 2500m);
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            RefNumber = "S20260925001",
            SaleDate = DateTime.UtcNow,
            CustomerName = "Walk in",
            Total = 5000m,
            AmountPaid = 3000m,
            BalanceDue = 2000m,
            Status = SaleStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        sale.Lines.Add(new SaleLine
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityKg = 2m,
            UnitPrice = 2500m,
            LineTotal = 2m * 2500m
        });
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

        var saved = await db.SaleLines.SingleAsync(x => x.Id == sale.Lines.First().Id);
        Assert.AreEqual(5000m, saved.LineTotal);
        Assert.AreEqual(5000m, sale.Total);
        Assert.AreEqual(2000m, sale.Total - sale.AmountPaid);
        Assert.AreEqual(sale.BalanceDue, sale.Total - sale.AmountPaid);
    }

    [TestMethod]
    public async Task DealTimePrice_StaysFixed_WhenCurrentPriceMoves()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("priceowner");
        var beans = NewCommodity("BeansPrice", 3000m);
        db.Users.Add(user);
        db.Commodities.Add(beans);
        await db.SaveChangesAsync();

        var line = new SaleLine
        {
            Id = Guid.NewGuid(),
            CommodityId = beans.Id,
            QuantityKg = 5m,
            UnitPrice = 2800m,
            LineTotal = 5m * 2800m
        };
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            RefNumber = "S20260925002",
            SaleDate = DateTime.UtcNow,
            CustomerName = "Walk in",
            Total = 14000m,
            AmountPaid = 14000m,
            BalanceDue = 0m,
            Status = SaleStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        sale.Lines.Add(line);
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

        beans.CurrentPrice = 3500m;
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var kept = await db.SaleLines.SingleAsync(x => x.Id == line.Id);
        Assert.AreEqual(2800m, kept.UnitPrice);
        Assert.AreEqual(14000m, kept.LineTotal);
    }

    [TestMethod]
    public async Task HappyPath_PurchasePlusSalePlusPayment_AllMatch()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("shopowner");
        var maize = NewCommodity("MaizeHappy", 2000m);
        var beans = NewCommodity("BeansHappy", 3000m);
        db.Users.Add(user);
        db.Commodities.AddRange(maize, beans);
        await db.SaveChangesAsync();

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            RefNumber = "P20260925001",
            PurchaseDate = DateTime.UtcNow,
            SupplierName = "Walk in supplier",
            Total = 350000m,
            AmountPaid = 200000m,
            BalanceDue = 150000m,
            Status = PurchaseStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        purchase.Lines.Add(new PurchaseLine
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityKg = 100m,
            UnitPrice = 2000m,
            LineTotal = 200000m
        });
        purchase.Lines.Add(new PurchaseLine
        {
            Id = Guid.NewGuid(),
            CommodityId = beans.Id,
            QuantityKg = 50m,
            UnitPrice = 3000m,
            LineTotal = 150000m
        });
        db.Purchases.Add(purchase);
        await db.SaveChangesAsync();

        db.Payments.Add(new Payment
        {
            Id = Guid.NewGuid(),
            PurchaseId = purchase.Id,
            Amount = 200000m,
            PaymentDate = DateTime.UtcNow,
            ReceivedByUserId = user.Id,
            Method = "Cash"
        });
        db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            MovementType = StockMovementType.In,
            QuantityKg = 100m,
            MovementDate = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            PurchaseId = purchase.Id
        });
        db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            CommodityId = beans.Id,
            MovementType = StockMovementType.In,
            QuantityKg = 50m,
            MovementDate = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            PurchaseId = purchase.Id
        });
        await db.SaveChangesAsync();

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            RefNumber = "S20260925003",
            SaleDate = DateTime.UtcNow,
            CustomerName = "Walk in buyer",
            Total = 25000m,
            AmountPaid = 10000m,
            BalanceDue = 15000m,
            Status = SaleStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        sale.Lines.Add(new SaleLine
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityKg = 5m,
            UnitPrice = 2000m,
            LineTotal = 10000m
        });
        sale.Lines.Add(new SaleLine
        {
            Id = Guid.NewGuid(),
            CommodityId = beans.Id,
            QuantityKg = 5m,
            UnitPrice = 3000m,
            LineTotal = 15000m
        });
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

        db.Payments.Add(new Payment
        {
            Id = Guid.NewGuid(),
            SaleId = sale.Id,
            Amount = 10000m,
            PaymentDate = DateTime.UtcNow,
            ReceivedByUserId = user.Id,
            Method = "Cash"
        });
        db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            MovementType = StockMovementType.Out,
            QuantityKg = 5m,
            MovementDate = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            SaleId = sale.Id
        });
        db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            CommodityId = beans.Id,
            MovementType = StockMovementType.Out,
            QuantityKg = 5m,
            MovementDate = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            SaleId = sale.Id
        });
        await db.SaveChangesAsync();

        var purchaseLines = await db.PurchaseLines.Where(x => x.PurchaseId == purchase.Id).ToListAsync();
        var saleLines = await db.SaleLines.Where(x => x.SaleId == sale.Id).ToListAsync();
        var salePayments = await db.Payments.Where(x => x.SaleId == sale.Id).ToListAsync();
        var purchasePayments = await db.Payments.Where(x => x.PurchaseId == purchase.Id).ToListAsync();
        var moves = await db.StockMovements.ToListAsync();

        Assert.HasCount(2, purchaseLines);
        Assert.HasCount(2, saleLines);
        Assert.AreEqual(350000m, purchaseLines.Sum(x => x.LineTotal));
        Assert.AreEqual(25000m, saleLines.Sum(x => x.LineTotal));
        Assert.AreEqual(10000m, salePayments.Sum(x => x.Amount));
        Assert.AreEqual(200000m, purchasePayments.Sum(x => x.Amount));
        Assert.AreEqual(15000m, sale.Total - sale.AmountPaid);
        Assert.HasCount(4, moves);
    }

    [TestMethod]
    public async Task Sale_LinkedCustomer_KeepsLinkAndSnapshot()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("linkowner");
        var maize = NewCommodity("MaizeLink", 2000m);
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "School buyer",
            Phone = "0700000001",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Users.Add(user);
        db.Commodities.Add(maize);
        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        // AC-5: the link stays optional and the name copy stays frozen.
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            RefNumber = "S20260925020",
            SaleDate = DateTime.UtcNow,
            CustomerId = customer.Id,
            CustomerName = "School buyer Gate",
            Total = 10000m,
            AmountPaid = 10000m,
            BalanceDue = 0m,
            Status = SaleStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        sale.Lines.Add(new SaleLine
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityKg = 5m,
            UnitPrice = 2000m,
            LineTotal = 10000m
        });
        db.Sales.Add(sale);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var saved = await db.Sales
            .Include(x => x.Customer)
            .SingleAsync(x => x.Id == sale.Id);
        Assert.AreEqual(customer.Id, saved.CustomerId);
        Assert.AreEqual("School buyer", saved.Customer!.Name);
        Assert.AreEqual("School buyer Gate", saved.CustomerName);
    }

    [TestMethod]
    public async Task Purchase_LinkedSupplier_KeepsLinkAndSnapshot()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("supplyowner");
        var maize = NewCommodity("MaizeSupply", 2000m);
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = "Village agent",
            Phone = "0700000002",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Users.Add(user);
        db.Commodities.Add(maize);
        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync();

        // AC-5: the link stays optional and the name copy stays frozen.
        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            RefNumber = "P20260925020",
            PurchaseDate = DateTime.UtcNow,
            SupplierId = supplier.Id,
            SupplierName = "Village agent Market",
            Total = 20000m,
            AmountPaid = 20000m,
            BalanceDue = 0m,
            Status = PurchaseStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        purchase.Lines.Add(new PurchaseLine
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityKg = 10m,
            UnitPrice = 2000m,
            LineTotal = 20000m
        });
        db.Purchases.Add(purchase);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var saved = await db.Purchases
            .Include(x => x.Supplier)
            .SingleAsync(x => x.Id == purchase.Id);
        Assert.AreEqual(supplier.Id, saved.SupplierId);
        Assert.AreEqual("Village agent", saved.Supplier!.Name);
        Assert.AreEqual("Village agent Market", saved.SupplierName);
    }

    [TestMethod]
    public async Task Adjustment_Loss_SavesWithLinkedMovement()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("lossowner");
        var maize = NewCommodity("MaizeLoss", 2000m, 100m);
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        // A loss stores negative on the fix and positive on the ledger row.
        var fix = new StockAdjustment
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityChangeKg = -3m,
            Reason = "Spoilage",
            CreatedByUserId = user.Id,
            AdjustmentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        db.StockAdjustments.Add(fix);
        await db.SaveChangesAsync();

        db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            MovementType = StockMovementType.Adjustment,
            QuantityKg = 3m,
            MovementDate = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            Reason = "Spoilage",
            StockAdjustmentId = fix.Id
        });
        await db.SaveChangesAsync();

        var move = await db.StockMovements.SingleAsync(x => x.StockAdjustmentId == fix.Id);
        Assert.AreEqual(StockMovementType.Adjustment, move.MovementType);
        Assert.AreEqual(3m, move.QuantityKg);
        Assert.AreEqual(-3m, fix.QuantityChangeKg);
    }
}
