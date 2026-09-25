# Business Rules

## 1. Database source of truth

The SQLite database is the authoritative source for business data.

## 2. Offline operation

The application is intended to operate without cloud services or internet connectivity.

## 3. Historical pricing

Historical transactions preserve the actual price at the time of the transaction.

**Never reconstruct an old transaction using the commodity's current price.**

A sale/purchase must therefore retain its transaction-time price.

## 4. Partial payments

Sales track these separately:

- Total
- Amount paid
- Balance due

A sale may be partially paid.

Purchases similarly track:

- Total
- Amount paid
- Balance

## 5. Stock movements

### Purchase

A purchase creates:

```text
Stock IN
```

### Sale

A sale creates:

```text
Stock OUT
```

### Manual adjustment

A controlled manual adjustment creates a stock movement representing the adjustment.

## 6. Stock ledger audit trail

Every:

- Purchase
- Sale
- Manual stock adjustment

creates a corresponding stock-movement record.

The Stock Ledger is therefore an audit trail of stock changes.

## 7. Ledger editing

Users should not edit stock ledger rows directly to change stock.

Instead:

```text
Controlled stock adjustment
        ↓
Stock quantity change
        ↓
New ledger movement
```

This preserves the audit trail.

## 8. Quantity storage

The database base unit is **kg**.

Store quantities in kilograms.

Bags and tonnes are display conversions only and must not replace kg as the stored base unit.

## 9. Quantity display

- Always show `kg` explicitly.
- Use thousands separators.
- Avoid unnecessary decimal places.

Examples:

```text
250 kg
1,250 kg
12,500 kg
```

## 10. Currency

Currency is UGX.

Use:

```text
UGX 125,000
```

Rules:

- UGX prefix.
- Thousands separators.
- Whole shillings by default.
- No unnecessary decimals.

## 11. Stock statuses

Use these statuses:

- In Stock
- Low Stock
- Out of Stock

The exact threshold logic should be based on the commodity's configured reorder/stock thresholds.

## 12. Receipt

A sale should have a printable receipt.

The print layout should be separate from the normal application UI.

The exact paper format—thermal versus A4—has not been decided in the available requirements.

## 13. Permission enforcement

Role permissions must be enforced server-side/data-access-side, not only through UI visibility.

## 14. Historical integrity

Operations that affect historical stock or financial records should preserve an auditable history.

This is why posted sales and purchases should use controlled correction/reversal behavior rather than unrestricted destructive deletion.
