# Screens

## Core screens

### 1. Login

**Purpose:** Authenticate a local application user.

**Capabilities:**
- Enter credentials.
- Authenticate against local user records.
- Start a local session.

**CRUD:** Authentication/read only.

---

### 2. Dashboard

**Purpose:** Give staff a compact operational overview.

**Primary behavior:** Read-only aggregation of current business information.

**CRUD:** Read only.

**Likely information areas:**
- Sales summary
- Purchase summary
- Stock summary
- Low-stock items
- Outstanding balances

The exact dashboard metrics should follow the approved UI specification rather than inventing additional metrics.

---

### 3. Commodities

**Purpose:** Maintain the commodities/products that the business buys and sells.

**Screens / views:**
- Commodity list
- Add commodity
- Edit commodity
- Commodity details

**CRUD:** Create, read, update, delete, subject to historical-reference constraints.

**Important rule:** A commodity referenced by historical transactions must not be deleted in a way that breaks transaction history.

---

### 4. Sales

**Purpose:** Record and review customer sales.

**Screens / views:**
- Sales list
- New sale
- Sale details
- Printable receipt

**CRUD:** Create/read; controlled update/correction/reversal for posted transactions.

**Transaction data includes:**
- Commodity
- Quantity
- Actual sale price
- Total
- Amount paid
- Balance due

Sales may be partially paid.

Every sale creates a stock OUT movement.

---

### 5. Purchases

**Purpose:** Record purchases that bring commodities into stock.

**Screens / views:**
- Purchase list
- New purchase
- Purchase details

**CRUD:** Create/read; controlled update/correction/reversal for posted transactions.

**Transaction data includes:**
- Commodity
- Quantity
- Actual purchase price
- Total
- Amount paid
- Balance

Purchases create stock IN movements.

---

### 6. Stock Ledger

**Purpose:** Provide an auditable history of stock movement.

**CRUD:** Read only for ledger rows.

**Movement sources:**
- Purchase → IN
- Sale → OUT
- Manual adjustment → adjustment movement

A ledger row should not be directly edited as a way of changing stock.

Stock adjustments are controlled operations that create a corresponding ledger entry.

---

## Supporting screens

### Reports

**Purpose:** Provide business reporting.

Reports are read-only/generated views.

Role access:
- Owner: yes
- Manager: yes
- Cashier: no

The source requirements explicitly imply reports, but the exact report catalog is not fully specified.

Possible report areas previously identified:
- Sales
- Purchases
- Stock
- Balances

These should be treated as planned areas until confirmed.

### Users

**Purpose:** Local user management.

Role access:
- Owner: yes
- Manager: yes
- Cashier: no

The source requires user management but does not fully specify whether deletion should be hard delete, deactivation, or another lifecycle.

### Account/Profile

Supporting screen for the signed-in user's own account information.

This is recommended/supporting rather than one of the six confirmed core screens.

### Settings

Supporting application/business configuration area.

This is recommended/supporting rather than one of the six confirmed core screens.

## Screen navigation

```text
Login
  └── Dashboard
      ├── Sales
      │   ├── Sales List
      │   ├── New Sale
      │   ├── Sale Details
      │   └── Receipt
      ├── Purchases
      │   ├── Purchase List
      │   ├── New Purchase
      │   └── Purchase Details
      ├── Commodities
      │   ├── Commodity List
      │   ├── Add Commodity
      │   ├── Edit Commodity
      │   └── Commodity Details
      ├── Stock Ledger
      │   └── Stock Adjustment
      ├── Reports
      ├── Users
      └── Settings / Account
```
