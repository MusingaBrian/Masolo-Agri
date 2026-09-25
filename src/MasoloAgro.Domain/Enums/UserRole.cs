namespace MasoloAgro.Domain.Enums;

/// <summary>
/// Local user roles. Authorization is enforced in the C# application layer,
/// never by hiding UI controls alone.
/// </summary>
public enum UserRole
{
    Owner = 0,
    Manager = 1,
    Cashier = 2
}
