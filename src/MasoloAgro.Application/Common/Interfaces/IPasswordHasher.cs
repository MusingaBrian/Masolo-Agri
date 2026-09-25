namespace MasoloAgro.Application.Common.Interfaces;

/// <summary>
/// Hashes and verifies local user passwords. Plaintext passwords must never
/// be stored or returned.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
