using MasoloAgro.Domain.Enums;

namespace MasoloAgro.Domain.Entities;

/// <summary>
/// Local app user. Password stays hashed, plain text never stored.
/// One user owns many sessions plus deals plus moves created by them.
/// </summary>
public sealed class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Cashier;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
}
