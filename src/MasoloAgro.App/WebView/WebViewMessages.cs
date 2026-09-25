namespace MasoloAgro.App.WebView;

/// <summary>
/// Operation names for the typed WebView bridge. The React frontend may only
/// invoke these explicit operations; arbitrary C# methods and the DbContext
/// are never exposed to JavaScript. Operations are implemented feature by
/// feature; this file keeps the contract in one clearly defined place.
/// </summary>
public static class BridgeOperations
{
    public const string AuthLogin = "auth.login";
    public const string AuthLogout = "auth.logout";
    public const string AuthCurrentSession = "auth.currentSession";

    public const string CommoditiesList = "commodities.list";
    public const string CommoditiesGet = "commodities.get";
    public const string CommoditiesCreate = "commodities.create";
    public const string CommoditiesUpdate = "commodities.update";

    public const string SalesList = "sales.list";
    public const string SalesGet = "sales.get";
    public const string SalesCreate = "sales.create";
    public const string SalesReverse = "sales.reverse";

    public const string PurchasesList = "purchases.list";
    public const string PurchasesGet = "purchases.get";
    public const string PurchasesCreate = "purchases.create";
    public const string PurchasesReverse = "purchases.reverse";

    public const string StockSummary = "stock.summary";
    public const string StockLedger = "stock.ledger";
    public const string StockAdjust = "stock.adjust";

    public const string ReportsSales = "reports.sales";
    public const string ReportsPurchases = "reports.purchases";
    public const string ReportsStock = "reports.stock";

    public const string UsersList = "users.list";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDisable = "users.disable";
}

/// <summary>
/// Structured request sent from the React frontend. Payload carries
/// operation-specific JSON, or null when the operation takes no input.
/// </summary>
public sealed record BridgeRequest(string Operation, string? Payload);

/// <summary>
/// Structured result returned to the React frontend. Failures carry a short
/// user-facing message only; stack traces, SQL, and secrets never cross
/// the bridge.
/// </summary>
public sealed record BridgeResponse(bool Ok, string? Payload, string? Error);
