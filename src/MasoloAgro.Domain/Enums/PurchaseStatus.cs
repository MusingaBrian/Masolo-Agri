namespace MasoloAgro.Domain.Enums;

/// <summary>
/// Purchase life cycle. A posted purchase stays in place and a fix moves
/// it to reversed with a reversing movement plus a reason.
/// </summary>
public enum PurchaseStatus
{
    Posted = 0,
    Reversed = 1
}
