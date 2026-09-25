# User Flows

## Login flow

```text
Open application
    ↓
Login screen
    ↓
Enter local credentials
    ↓
Authenticate user
    ↓
Load role/session
    ↓
Dashboard
```

## Sale flow

```text
Sales
  ↓
New Sale
  ↓
Select commodity
  ↓
Enter quantity (kg)
  ↓
Enter transaction-time sale price
  ↓
Calculate total
  ↓
Enter amount paid
  ↓
Calculate balance due
  ↓
Save sale
  ↓
Create Stock OUT movement
  ↓
Sale details
  ↓
Optional printable receipt
```

## Partial payment flow

```text
Sale total
    ↓
Amount paid < total
    ↓
Balance due recorded
    ↓
Sale remains partially paid
```

The exact workflow for later balance settlement is not fully specified and should be defined before implementation if required.

## Purchase flow

```text
Purchases
  ↓
New Purchase
  ↓
Select commodity
  ↓
Enter quantity (kg)
  ↓
Enter transaction-time purchase price
  ↓
Calculate total
  ↓
Enter amount paid
  ↓
Calculate balance
  ↓
Save purchase
  ↓
Create Stock IN movement
  ↓
Purchase details
```

## Stock adjustment flow

```text
Stock Ledger
    ↓
Create Stock Adjustment
    ↓
Select commodity
    ↓
Enter adjustment quantity
    ↓
Record reason / adjustment information
    ↓
Save adjustment
    ↓
Update stock
    ↓
Create adjustment ledger movement
```

The exact adjustment fields and approval requirements are not specified in the source requirements.

## Commodity flow

```text
Commodities
   ├── Add Commodity
   ├── View Commodity
   └── Edit Commodity
```

Deletion should be constrained when historical transactions reference the commodity.

## User-management flow

```text
Users
   ├── Add User
   ├── View User
   └── Edit User
```

Exact deactivation/deletion behavior remains to be decided.

## Reporting flow

```text
Reports
   ↓
Select report area
   ↓
Apply available filters/date range
   ↓
Read generated report
```

The exact report catalog and filters are not fully specified.

## Core stock lifecycle

```text
Purchase
   ↓
Stock IN
   ↓
Available stock
   ↓
Sale
   ↓
Stock OUT
```

A manual adjustment is a separate controlled movement in the same audit trail.
