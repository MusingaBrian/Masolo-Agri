# Data Model

This document captures the domain entities and invariants supported by the requirements. It is intentionally not a full Drizzle schema.

## User

Represents a local application user.

Core role values:

```text
Owner
Manager
Cashier
```

Authentication uses local sessions and hashed passwords.

## Commodity

Represents an agricultural commodity managed by the business.

Important characteristics:

- Has current commodity information.
- Has stock quantity.
- Has stock/reorder status.
- Is referenced by sales and purchases.
- Must remain historically referentially safe.

## Sale

Represents a customer sale.

Important fields/concepts:

- Commodity
- Quantity in kg
- Transaction-time sale price
- Total
- Amount paid
- Balance due
- Transaction history

A sale creates a Stock OUT movement.

## Purchase

Represents a purchase from a supplier.

Important fields/concepts:

- Commodity
- Quantity in kg
- Transaction-time purchase price
- Total
- Amount paid
- Balance
- Transaction history

A purchase creates a Stock IN movement.

## Stock Movement

Represents an auditable change to stock.

Movement sources include:

```text
Purchase → IN
Sale → OUT
Manual Adjustment → Adjustment
```

The ledger should preserve the movement history.

## Stock Adjustment

Represents a controlled manual stock correction.

It should not mutate ledger rows directly.

Instead:

```text
Adjustment operation
      ↓
Stock change
      ↓
New Stock Movement
```

## Relationships

```text
User
  └── local authentication/session

Commodity
  ├── Sales
  ├── Purchases
  └── Stock Movements

Purchase
  └── Stock Movement (IN)

Sale
  └── Stock Movement (OUT)

Stock Adjustment
  └── Stock Movement (Adjustment)
```

## Data invariants

1. Stored quantity uses kg as the base unit.
2. Historical transaction prices are stored with the transaction.
3. Current commodity pricing must not be used to reconstruct historical transactions.
4. Every purchase creates stock IN.
5. Every sale creates stock OUT.
6. Every manual adjustment creates an auditable movement.
7. Ledger records are not directly edited.
8. Sales maintain total, paid, and balance separately.
9. Purchases maintain total, paid, and balance separately.

## Implementation note

The exact columns, IDs, indexes, foreign keys, timestamps, payment records, and audit metadata should be defined in the Drizzle schema before implementation. This document does not invent fields that were not specified.

## Persistence implementation

SQLite is the local database and source of truth. Entity Framework Core with `Microsoft.EntityFrameworkCore.Sqlite` is the planned C# data-access layer.
