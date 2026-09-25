using MasoloAgro.Application.Common.Interfaces;
using MasoloAgro.Infrastructure.Database;
using MasoloAgro.Infrastructure.Security;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MasoloAgro.Tests;

/// <summary>
/// Foundation smoke tests. These prove the test project, the password
/// hasher, and the EF Core SQLite wiring execute. Business tests arrive
/// with each feature.
/// </summary>
[TestClass]
public sealed class FoundationSmokeTests
{
    [TestMethod]
    public void PasswordHasher_RoundTrips()
    {
        IPasswordHasher hasher = new Pbkdf2PasswordHasher();

        var hash = hasher.Hash("smoke-test-password");

        Assert.IsTrue(hasher.Verify("smoke-test-password", hash));
        Assert.IsFalse(hasher.Verify("wrong-password", hash));
    }

    [TestMethod]
    public void AppDbContext_ConnectsOverSqlite()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);

        Assert.IsTrue(context.Database.CanConnect());
    }
}
