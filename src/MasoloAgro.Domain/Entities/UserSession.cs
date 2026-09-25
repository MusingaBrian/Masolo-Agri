namespace MasoloAgro.Domain.Entities;

/// <summary>
/// Stored sign in session. Token proves sign in and expires by time
/// or by explicit revoke.
/// </summary>
public sealed class UserSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User? User { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }
}
