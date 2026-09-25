namespace MasoloAgro.Domain.Enums;

/// <summary>
/// Sale life cycle. A posted sale stays in place and a fix moves it
/// to reversed with a reversing movement plus a reason.
/// </summary>
public enum SaleStatus
{
    Posted = 0,
    Reversed = 1
}
