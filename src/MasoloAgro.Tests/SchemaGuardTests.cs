using MasoloAgro.Application.Common.Interfaces;
using MasoloAgro.Domain.Entities;
using MasoloAgro.Domain.Enums;
using MasoloAgro.Infrastructure.Database;
using MasoloAgro.Infrastructure.Security;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MasoloAgro.Tests;

/// <summary>
/// Guard proof for uniques plus ranges plus safe deletes plus reversal
/// plus rollback plus seed plus sessions. Covers AC-6 plus AC-7 plus AC-8
/// plus AC-9 from spec 0001.
/// </summary>
[TestClass]
public sealed class SchemaGuardTests
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

    private static User NewUser(string username)
    {
        var now = DateTime.UtcNow;
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = "hash",
            Role = UserRole.Owner,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private static Commodity NewCommodity(string name)
    {
        var now = DateTime.UtcNow;
        return new Commodity
        {
            Id = Guid.NewGuid(),
            Name = name,
            CurrentPrice = 1000m,
            StockKg = 0,
            ReorderLevelKg = 5,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private static Sale NewSale(User user, string refNumber, decimal total, decimal paid)
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            RefNumber = refNumber,
            SaleDate = DateTime.UtcNow,
            CustomerName = "Walk in",
            Total = total,
            AmountPaid = paid,
            BalanceDue = total - paid,
            Status = SaleStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static async Task ExpectDbUpdateAsync(Func<Task<int>> action)
    {
        try
        {
            await action();
        }
        catch (DbUpdateException)
        {
            return;
        }

        Assert.Fail("Expected a database guard failure.");
    }

    [TestMethod]
    public async Task Unique_Username_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        db.Users.Add(NewUser("sameuser"));
        await db.SaveChangesAsync();

        db.Users.Add(NewUser("sameuser"));
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Unique_CommodityName_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        db.Commodities.Add(NewCommodity("SameCrop"));
        await db.SaveChangesAsync();

        db.Commodities.Add(NewCommodity("SameCrop"));
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Unique_SaleRef_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("refowner");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        db.Sales.Add(NewSale(user, "S20260925010", 1000m, 1000m));
        await db.SaveChangesAsync();

        db.Sales.Add(NewSale(user, "S20260925010", 500m, 0m));
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Unique_SessionToken_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("tokenowner");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var now = DateTime.UtcNow;
        db.UserSessions.Add(new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "token123",
            CreatedAt = now,
            ExpiresAt = now.AddHours(8)
        });
        await db.SaveChangesAsync();

        db.UserSessions.Add(new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "token123",
            CreatedAt = now,
            ExpiresAt = now.AddHours(8)
        });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Line_ZeroQuantity_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("qtyowner");
        var maize = NewCommodity("MaizeQty");
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        var sale = NewSale(user, "S20260925011", 1000m, 0m);
        sale.Lines.Add(new SaleLine
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityKg = 0m,
            UnitPrice = 1000m,
            LineTotal = 0m
        });
        db.Sales.Add(sale);
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Sale_PaidAboveTotal_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("paidowner");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        db.Sales.Add(NewSale(user, "S20260925012", 1000m, 1500m));
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Payment_ZeroAmount_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("payowner");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var sale = NewSale(user, "S20260925013", 1000m, 0m);
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

        db.Payments.Add(new Payment
        {
            Id = Guid.NewGuid(),
            SaleId = sale.Id,
            Amount = 0m,
            PaymentDate = DateTime.UtcNow,
            ReceivedByUserId = user.Id
        });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Payment_RequiresExactlyOneParent()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("parentowner");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        db.Payments.Add(new Payment
        {
            Id = Guid.NewGuid(),
            Amount = 500m,
            PaymentDate = DateTime.UtcNow,
            ReceivedByUserId = user.Id
        });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
        db.ChangeTracker.Clear();

        var sale = NewSale(user, "S20260925014", 1000m, 0m);
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            RefNumber = "P20260925014",
            PurchaseDate = DateTime.UtcNow,
            SupplierName = "Walk in",
            Total = 1000m,
            AmountPaid = 0m,
            BalanceDue = 1000m,
            Status = PurchaseStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        db.Purchases.Add(purchase);
        await db.SaveChangesAsync();

        db.Payments.Add(new Payment
        {
            Id = Guid.NewGuid(),
            SaleId = sale.Id,
            PurchaseId = purchase.Id,
            Amount = 100m,
            PaymentDate = DateTime.UtcNow,
            ReceivedByUserId = user.Id
        });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Movement_RejectsTwoSources()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("moveowner");
        var maize = NewCommodity("MaizeMove");
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        var sale = NewSale(user, "S20260925015", 1000m, 0m);
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            RefNumber = "P20260925015",
            PurchaseDate = DateTime.UtcNow,
            SupplierName = "Walk in",
            Total = 1000m,
            AmountPaid = 0m,
            BalanceDue = 1000m,
            Status = PurchaseStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        db.Purchases.Add(purchase);
        await db.SaveChangesAsync();

        db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            MovementType = StockMovementType.Out,
            QuantityKg = 2m,
            MovementDate = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            SaleId = sale.Id,
            PurchaseId = purchase.Id
        });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Adjustment_ZeroChange_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("adjustowner");
        var maize = NewCommodity("MaizeAdjust");
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        db.StockAdjustments.Add(new StockAdjustment
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityChangeKg = 0m,
            Reason = "Recount",
            CreatedByUserId = user.Id,
            AdjustmentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task UsedCommodity_Delete_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("deleteowner");
        var maize = NewCommodity("MaizeUsed");
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        var sale = NewSale(user, "S20260925016", 1000m, 0m);
        sale.Lines.Add(new SaleLine
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityKg = 1m,
            UnitPrice = 1000m,
            LineTotal = 1000m
        });
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

        // Clear tracked rows so the database itself must block the delete.
        var commodityId = maize.Id;
        db.ChangeTracker.Clear();

        db.Commodities.Remove(new Commodity { Id = commodityId });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Customer_Delete_Blocked_WhenSalesExist()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("customerowner");
        db.Users.Add(user);
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "School buyer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        var sale = NewSale(user, "S20260925017", 1000m, 0m);
        sale.CustomerId = customer.Id;
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

        // Clear tracked rows so the database itself must block the delete.
        var customerId = customer.Id;
        db.ChangeTracker.Clear();

        db.Customers.Remove(new Customer { Id = customerId });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Reversal_KeepsOldRows_AndAddsRestoringMovement()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("reverseowner");
        var maize = NewCommodity("MaizeReverse");
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        var sale = NewSale(user, "S20260925018", 5000m, 5000m);
        sale.Lines.Add(new SaleLine
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityKg = 5m,
            UnitPrice = 1000m,
            LineTotal = 5000m
        });
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

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
        await db.SaveChangesAsync();

        sale.Status = SaleStatus.Reversed;
        sale.ReversedAt = DateTime.UtcNow;
        sale.ReversalReason = "Wrong item";
        db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            MovementType = StockMovementType.In,
            QuantityKg = 5m,
            MovementDate = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            Reason = "Reversal of S20260925018"
        });
        await db.SaveChangesAsync();

        var moves = await db.StockMovements.Where(x => x.CommodityId == maize.Id).ToListAsync();
        var lines = await db.SaleLines.Where(x => x.SaleId == sale.Id).ToListAsync();
        Assert.HasCount(2, moves);
        Assert.HasCount(1, lines);
        Assert.AreEqual(SaleStatus.Reversed, sale.Status);
    }

    [TestMethod]
    public async Task Rollback_LeavesNoPartialRows()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("rollbackowner");
        var maize = NewCommodity("MaizeRollback");
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            var sale = NewSale(user, "S20260925019", 1000m, 0m);
            db.Sales.Add(sale);
            await db.SaveChangesAsync();

            db.SaleLines.Add(new SaleLine
            {
                Id = Guid.NewGuid(),
                SaleId = sale.Id,
                CommodityId = maize.Id,
                QuantityKg = 0m,
                UnitPrice = 1000m,
                LineTotal = 0m
            });
            await db.SaveChangesAsync();

            await tx.CommitAsync();
            Assert.Fail("Expected a guard failure before commit.");
        }
        catch (DbUpdateException)
        {
            await tx.RollbackAsync();
        }

        db.ChangeTracker.Clear();
        Assert.AreEqual(0, await db.Sales.CountAsync());
        Assert.AreEqual(0, await db.SaleLines.CountAsync());
    }

    [TestMethod]
    public async Task Seed_CreatesOwnerOnce_AndHashVerifies()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        IPasswordHasher hasher = new Pbkdf2PasswordHasher();

        var first = await DbSeeder.EnsureOwnerAsync(db, hasher, "owner", "Secret123!");
        var second = await DbSeeder.EnsureOwnerAsync(db, hasher, "owner", "Secret123!");

        Assert.AreEqual(first.Id, second.Id);
        Assert.AreEqual(1, await db.Users.CountAsync());
        Assert.AreEqual(UserRole.Owner, first.Role);
        Assert.IsTrue(first.IsActive);
        Assert.IsTrue(hasher.Verify("Secret123!", first.PasswordHash));
        Assert.IsFalse(hasher.Verify("WrongPass1!", first.PasswordHash));
    }

    [TestMethod]
    public async Task Session_Expiry_IsReadable()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("sessionowner");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var now = DateTime.UtcNow;
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "expiredtoken1",
            CreatedAt = now.AddHours(-9),
            ExpiresAt = now.AddHours(-1)
        };
        db.UserSessions.Add(session);
        await db.SaveChangesAsync();

        var saved = await db.UserSessions.SingleAsync(x => x.Token == "expiredtoken1");
        Assert.IsTrue(saved.ExpiresAt < DateTime.UtcNow);
        Assert.IsNull(saved.RevokedAt);
    }

    [TestMethod]
    public async Task Supplier_Delete_Blocked_WhenPurchasesExist()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("supplierowner");
        db.Users.Add(user);
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = "Mill agent",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync();

        db.Purchases.Add(new Purchase
        {
            Id = Guid.NewGuid(),
            RefNumber = "P20260925021",
            PurchaseDate = DateTime.UtcNow,
            SupplierId = supplier.Id,
            SupplierName = "Mill agent",
            Total = 1000m,
            AmountPaid = 0m,
            BalanceDue = 1000m,
            Status = PurchaseStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        // AC-6: clear tracked rows so the database itself must block.
        var supplierId = supplier.Id;
        db.ChangeTracker.Clear();

        db.Suppliers.Remove(new Supplier { Id = supplierId });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Purchase_PaidAboveTotal_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("buyoverpaid");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        db.Purchases.Add(new Purchase
        {
            Id = Guid.NewGuid(),
            RefNumber = "P20260925022",
            PurchaseDate = DateTime.UtcNow,
            SupplierName = "Walk in",
            Total = 1000m,
            AmountPaid = 1500m,
            BalanceDue = -500m,
            Status = PurchaseStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task PurchaseLine_ZeroQuantity_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("buyqtyowner");
        var maize = NewCommodity("MaizeBuyQty");
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            RefNumber = "P20260925023",
            PurchaseDate = DateTime.UtcNow,
            SupplierName = "Walk in",
            Total = 1000m,
            AmountPaid = 0m,
            BalanceDue = 1000m,
            Status = PurchaseStatus.Posted,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        purchase.Lines.Add(new PurchaseLine
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityKg = 0m,
            UnitPrice = 1000m,
            LineTotal = 0m
        });
        db.Purchases.Add(purchase);
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Commodity_NegativePrice_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var bad = NewCommodity("MaizeBadPrice");
        bad.CurrentPrice = -50m;
        db.Commodities.Add(bad);
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task Movement_ZeroQuantity_Blocked()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("moveqtyowner");
        var maize = NewCommodity("MaizeMoveQty");
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            MovementType = StockMovementType.In,
            QuantityKg = 0m,
            MovementDate = DateTime.UtcNow,
            CreatedByUserId = user.Id
        });
        await ExpectDbUpdateAsync(() => db.SaveChangesAsync());
    }

    [TestMethod]
    public async Task PurchaseReversal_KeepsOldRows_AndAddsRestoringMovement()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("buyreverseowner");
        var maize = NewCommodity("MaizeBuyReverse");
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            RefNumber = "P20260925024",
            PurchaseDate = DateTime.UtcNow,
            SupplierName = "Walk in",
            Total = 10000m,
            AmountPaid = 10000m,
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
            UnitPrice = 1000m,
            LineTotal = 10000m
        });
        db.Purchases.Add(purchase);
        await db.SaveChangesAsync();

        db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            MovementType = StockMovementType.In,
            QuantityKg = 10m,
            MovementDate = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            PurchaseId = purchase.Id
        });
        await db.SaveChangesAsync();

        // AC-7: a purchase brought stock in, so reversal sends stock out.
        purchase.Status = PurchaseStatus.Reversed;
        purchase.ReversedAt = DateTime.UtcNow;
        purchase.ReversalReason = "Wrong weight";
        db.StockMovements.Add(new StockMovement
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            MovementType = StockMovementType.Out,
            QuantityKg = 10m,
            MovementDate = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            Reason = "Reversal of P20260925024"
        });
        await db.SaveChangesAsync();

        var moves = await db.StockMovements.Where(x => x.CommodityId == maize.Id).ToListAsync();
        var lines = await db.PurchaseLines.Where(x => x.PurchaseId == purchase.Id).ToListAsync();
        Assert.HasCount(2, moves);
        Assert.HasCount(1, lines);
        Assert.AreEqual(PurchaseStatus.Reversed, purchase.Status);
    }

    [TestMethod]
    public async Task SaleLines_FallWithParentSale()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("cascadeowner");
        var maize = NewCommodity("MaizeCascade");
        db.Users.Add(user);
        db.Commodities.Add(maize);
        await db.SaveChangesAsync();

        var sale = NewSale(user, "S20260925025", 1000m, 0m);
        sale.Lines.Add(new SaleLine
        {
            Id = Guid.NewGuid(),
            CommodityId = maize.Id,
            QuantityKg = 1m,
            UnitPrice = 1000m,
            LineTotal = 1000m
        });
        db.Sales.Add(sale);
        await db.SaveChangesAsync();

        // Draft cleanup: lines fall with the parent, history stays untouched.
        db.Sales.Remove(sale);
        await db.SaveChangesAsync();

        Assert.AreEqual(0, await db.Sales.CountAsync());
        Assert.AreEqual(0, await db.SaleLines.CountAsync());
        Assert.AreEqual(1, await db.Commodities.CountAsync());
    }

    [TestMethod]
    public async Task Sessions_FallWithDeletedUser()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using var db = CreateDb(connection);

        var user = NewUser("sessioncascade");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var now = DateTime.UtcNow;
        db.UserSessions.Add(new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "cascadetoken1",
            CreatedAt = now,
            ExpiresAt = now.AddHours(8)
        });
        await db.SaveChangesAsync();

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        Assert.AreEqual(0, await db.Users.CountAsync());
        Assert.AreEqual(0, await db.UserSessions.CountAsync());
    }
}
