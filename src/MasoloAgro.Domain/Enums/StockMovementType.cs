namespace MasoloAgro.Domain.Enums;

/// <summary>
/// Stock move direction. Quantity on the move stays positive and the type
/// tells which way stock went.
/// </summary>
public enum StockMovementType
{
    In = 0,
    Out = 1,
    Adjustment = 2
}
