using Microsoft.EntityFrameworkCore;

namespace MasoloAgro.Infrastructure.Database;

/// <summary>
/// Creates <see cref="AppDbContext"/> instances for EF Core design-time
/// tooling (migrations). Uses a dev database file so tooling never touches
/// the real business database.
/// </summary>
public sealed class AppDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(DatabaseLocation.GetDefaultConnectionString())
            .Options;

        return new AppDbContext(options);
    }
}
