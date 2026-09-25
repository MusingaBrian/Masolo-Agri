namespace MasoloAgro.App.Printing;

/// <summary>
/// Orchestrates sales receipt printing. The print-ready receipt layout
/// arrives with the sales feature; this type reserves the seam so features
/// do not invent their own printing path.
/// </summary>
public sealed class ReceiptPrinter
{
    public Task PrintReceiptAsync(string receiptReference, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(receiptReference);
        throw new NotImplementedException("Receipt printing arrives with the sales feature.");
    }
}
