using Microsoft.EntityFrameworkCore;

namespace MasoloAgro.Infrastructure.Database;

/// <summary>
/// EF Core context for the local offline SQLite database.
/// Entity sets are added feature by feature. This context is never
/// exposed to the frontend; all access goes through application services.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
