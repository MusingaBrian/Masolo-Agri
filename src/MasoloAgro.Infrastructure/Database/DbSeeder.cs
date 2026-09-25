using MasoloAgro.Application.Common.Interfaces;
using MasoloAgro.Domain.Entities;
using MasoloAgro.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MasoloAgro.Infrastructure.Database;

/// <summary>
/// Seeds the first Owner at runtime. Runs when the users table is empty
/// so the shop can open with a known sign in. Later sign ins use stored
/// users, never this default again.
/// </summary>
public static class DbSeeder
{
    public static async Task<User> EnsureOwnerAsync(
        AppDbContext db,
        IPasswordHasher hasher,
        string username = "owner",
        string password = "ChangeMe123!",
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(hasher);
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var existing = await db.Users.OrderBy(x => x.CreatedAt).FirstOrDefaultAsync(ct);
        if (existing is not null)
        {
            return existing;
        }

        var now = DateTime.UtcNow;
        var owner = new User
        {
            Id = Guid.NewGuid(),
            Username = username.Trim(),
            PasswordHash = hasher.Hash(password),
            Role = UserRole.Owner,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Users.Add(owner);
        await db.SaveChangesAsync(ct);
        return owner;
    }
}
