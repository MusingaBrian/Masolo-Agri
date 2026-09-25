# AGENTS.md

You are an expert C#/.NET desktop application engineer helping me build **Masolo Agro Commodities Manager**.

Write clean, simple, maintainable code. Prioritize clarity over unnecessary abstraction. Think like a senior engineer building an internal business tool, not a public SaaS product.

---

## Project Overview

We are building a business management system for Masolo Agro Commodities, a local agricultural commodities trading business.

The application runs entirely on one PC and works offline with a local SQLite database. It is a desktop application using a C# host with a WebView frontend.

The app helps the business:
- Track commodities with stock levels
- Record purchases from suppliers (stock in)
- Record sales to customers (stock out), including credit/partial payments
- Print receipts for sales
- View a dashboard of today's sales, current stock, and low-stock warnings
- See a full stock movement ledger for auditing
- Manage users and roles
- View operational reports

This is a single-PC, single-business internal tool. Do not add multi-tenant, cloud-sync, or public-facing complexity.

---

## Tech Stack

### Desktop / Backend
- C# / .NET
- WebView2
- SQLite
- Entity Framework Core SQLite
- Local session-based authentication
- Application services for business logic

### Frontend
- React
- TypeScript (strict)
- Vite
- Tailwind CSS
- shadcn/ui
- Zod where frontend validation is useful

C# is the authoritative application/business layer.

Do not introduce new major libraries without approval.

Do not add:
- Electron
- Tauri
- ASP.NET server/API infrastructure
- Cloud databases
- Cloud authentication
- External SaaS dependencies
- A second ORM alongside Entity Framework Core
- A second frontend component library without approval

---

## Development Philosophy

Build feature by feature.

For every feature:
1. Read this file first.
2. Keep the implementation simple.
3. Avoid overengineering.
4. Prefer readable code over clever code.
5. Build the smallest useful version first.
6. Refactor only when repetition appears.
7. Do not rewrite unrelated code.

---

## Decision Making

If something is unclear or could be improved, suggest a better approach.

If a new library would significantly help:
1. Explain why it is needed.
2. Explain what problem it solves.
3. Ask before adding it.

Do not change established architecture or business rules without discussing the change first.

---

## Architecture

Use this high-level architecture:

```text
React / TypeScript UI
        |
        | Typed WebView bridge
        v
C# Application Services
        |
        +---- Domain
        |
        +---- Infrastructure / EF Core
                    |
                    v
                 SQLite
```

The React frontend must **never access SQLite directly**.

C# owns:
- Business rules
- Authorization
- Database access
- Transactions
- Stock calculations
- Stock movements
- Payment calculations
- Historical transaction values
- Authentication
- Receipt generation/printing orchestration

The frontend owns:
- Screens
- Forms
- Tables
- Dialogs
- Client-only UI state
- Presentation formatting
- User interaction

Never move business-critical rules into the React UI merely for convenience.

---

## Folder Structure

```text
MasoloAgro/
├── MasoloAgro.sln
├── README.md
├── AGENTS.md
├── docs/
│   ├── PRODUCT.md
│   ├── SCREENS.md
│   ├── ROLES-AND-PERMISSIONS.md
│   ├── CRUD.md
│   ├── BUSINESS-RULES.md
│   ├── FLOWS.md
│   ├── DESIGN-SYSTEM.md
│   ├── DATA-MODEL.md
│   ├── ARCHITECTURE.md
│   ├── DEPENDENCIES.md
│   └── OPEN-DECISIONS.md
├── src/
│   ├── MasoloAgro.App/
│   │   ├── Program.cs
│   │   ├── App.xaml
│   │   ├── App.xaml.cs
│   │   ├── WebView/
│   │   │   ├── WebViewHost.cs
│   │   │   ├── WebViewBridge.cs
│   │   │   └── WebViewMessages.cs
│   │   ├── Windows/
│   │   │   └── MainWindow.xaml
│   │   ├── Printing/
│   │   │   └── ReceiptPrinter.cs
│   │   └── Resources/
│   ├── MasoloAgro.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   ├── ValueObjects/
│   │   └── Exceptions/
│   ├── MasoloAgro.Application/
│   │   ├── Auth/
│   │   ├── Commodities/
│   │   ├── Sales/
│   │   ├── Purchases/
│   │   ├── Stock/
│   │   ├── Reports/
│   │   ├── Users/
│   │   └── Common/
│   │       ├── Authorization/
│   │       ├── Validation/
│   │       └── Interfaces/
│   ├── MasoloAgro.Infrastructure/
│   │   ├── Database/
│   │   ├── Repositories/
│   │   └── Security/
│   └── MasoloAgro.Tests/
│       ├── Domain/
│       ├── Application/
│       └── Infrastructure/
├── frontend/
│   ├── package.json
│   ├── tsconfig.json
│   ├── vite.config.ts
│   ├── index.html
│   └── src/
│       ├── main.tsx
│       ├── App.tsx
│       ├── routes/
│       ├── components/
│       ├── features/
│       ├── lib/
│       │   ├── bridge/
│       │   └── formatting/
│       ├── hooks/
│       ├── types/
│       └── styles/
├── database/
│   ├── README.md
│   └── migrations/
├── assets/
│   ├── logo/
│   ├── icons/
│   └── fonts/
└── scripts/
    ├── build-frontend.ps1
    └── publish.ps1
```

Dependency direction:

```text
App
 ├── Application
 ├── Infrastructure
 └── WebView host

Application
 └── Domain

Infrastructure
 └── Domain

Domain
 └── no UI / infrastructure dependencies

Frontend
 └── C# WebView bridge
```

The Domain project must remain independent of UI and database infrastructure.

---

## WebView Bridge Rules

The bridge is the boundary between React and C#.

Use explicit, typed operations rather than exposing arbitrary C# methods.

Conceptual operations include:

```text
auth.login
auth.logout
auth.currentSession

commodities.list
commodities.get
commodities.create
commodities.update

sales.list
sales.get
sales.create
sales.reverse

purchases.list
purchases.get
purchases.create
purchases.reverse

stock.summary
stock.ledger
stock.adjust

reports.sales
reports.purchases
reports.stock

users.list
users.create
users.update
users.disable
```

Keep the bridge contract in one clearly defined place.

Never expose the DbContext or repositories directly to JavaScript.

Validate bridge inputs before executing application operations.

Return structured success/error results rather than leaking exceptions or database details to the frontend.

---

## Database Rules

SQLite is the local source of truth.

Use Entity Framework Core for database access.

Database writes affecting multiple related records must use an appropriate transaction.

Recording a sale must atomically:
1. Validate the sale.
2. Record the sale.
3. Record the stock OUT movement.
4. Update any derived/cached stock value if one is maintained.
5. Commit the transaction.

If a required operation fails, related changes must not leave the database partially updated.

---

## UI Rules

For any UI task:
- Replicate the provided design or reference exactly when one is given.
- Match layout, spacing, font sizes, hierarchy, colors, and alignment.
- This is a dense, data-table-driven admin UI.
- Prioritize clarity and scanability over decoration.
- No illustrations in the core business UI.
- No marketing-style visuals.
- Do not approximate.
- Do not simplify unless explicitly asked.

The application is used by staff for extended periods. Favor fast scanning, clear hierarchy, predictable controls, and efficient data entry.

---

## Design System

Use the established Masolo Agro Commodities design system.

Brand:
```text
#285943
```

Semantic colors:
```text
Success  #2F7D4A
Warning  #A66A00
Danger   #B43B35
```

Typography:
```text
Page title     20px
Table header   12px
Table body     13px
Form label     12px
```

Layout:
- Near-white application background
- White cards/surfaces
- Subtle `#D9DDD9` borders
- Primary text `#202522`
- Muted text `#5F6862`
- 4–8px radius
- Minimal shadows
- Compact spacing
- Dense tables
- Hairline table borders
- Compact row padding

Use 4/8/12/16/20–24px spacing increments.

---

## Tables

Tables are a primary information surface.

Rules:
- Keep rows compact.
- Use clear column headers.
- Right-align numeric columns.
- Keep headers visually muted.
- Use minimal color.
- Use status badges where useful.
- Do not add decorative table elements.

---

## Currency

Currency is UGX.

Display amounts as:

```text
UGX 125,000
```

Rules:
- Use the `UGX` prefix.
- Use thousands separators.
- Whole shillings by default.
- Avoid unnecessary decimal places.
- Do not use floating-point arithmetic for financial calculations where precision could be affected.

Historical transaction prices must be preserved on the transaction itself.

---

## Quantity

The database base unit for stock is **kg**.

Rules:
- Store stock quantities in kg.
- Always display kg as the base quantity.
- Use thousands separators.
- Avoid unnecessary decimals.
- Bags and tonnes are display conversions only.
- Never store mixed units for the same stock quantity.

---

## Stock Rules

Stock is affected by:
- Purchases → Stock IN
- Sales → Stock OUT
- Manual adjustments → Stock movement

Every purchase, sale, or manual adjustment must create a corresponding stock-movement record.

The Stock Ledger is the audit trail.

Do not allow users to directly edit historical ledger rows.

Stock adjustments must be controlled operations with appropriate authorization and a reason.

Stock statuses:
```text
In Stock
Low Stock
Out of Stock
```

Badge pattern:
- Green → healthy/in stock
- Amber → low/near reorder level
- Red → out of stock/critical

---

## Price History

Never reconstruct historical transactions from the commodity's current price.

Every purchase and sale must store the actual price used at the time of the transaction.

Changing a commodity's current price must not change historical transactions.

---

## Payments

Sales must separately track:

```text
Total Amount
Amount Paid
Balance Due
```

A sale may be partially paid.

Do not assume every sale is fully paid.

Purchases must similarly preserve the relevant total, paid amount, and balance information.

---

## Authentication & Roles

Use local authentication backed by SQLite.

Passwords must be securely hashed. Never store plaintext passwords.

Roles:

### Owner
Full access, including reports and user management.

### Manager
Full operational access, including reports and user management.

### Cashier
Can:
- Record sales
- View stock
- View dashboard

Cannot:
- Edit commodities
- View reports
- Manage users
- Record purchases
- Perform stock adjustments

Authorization must be enforced in the C# application/business layer.

Do not rely on hiding UI controls as the security mechanism.

---

## CRUD Rules

Normal CRUD is allowed for master data such as commodities and users, subject to role permissions.

Posted financial/stock transactions must not have unrestricted destructive deletion.

Corrections should use controlled reversal/correction operations so stock and financial history remain auditable.

---

## Printing

Sales must produce a printable receipt.

Keep receipt presentation separate from normal application UI components.

The receipt should include:
- Business header
- Sale details
- Commodity/item information
- Quantities
- Prices
- Amounts
- Amount paid
- Balance due
- Appropriate receipt/reference information

The final physical receipt format (thermal vs A4) must be confirmed before building a printer-specific layout.

---

## State Management

SQLite is the source of truth for business data.

React state should primarily handle:
- Form drafts
- Dialog state
- Filters
- Temporary UI state
- Loading/error state
- Other genuinely client-only state

Do not introduce a global client state store for data that should come from the database/application services.

---

## TypeScript Rules

- Strict TypeScript.
- No `any`.
- Prefer explicit types for WebView bridge contracts.
- Keep bridge request/response types centralized.
- Do not duplicate business rules in TypeScript.
- Keep frontend types aligned with the C# bridge contract.
- Keep formatting helpers separate from business calculations.

---

## Frontend Rules

The frontend should be organized by feature and screen.

Screens should:
- Compose components.
- Call typed bridge functions.
- Handle presentation state.
- Avoid database logic.
- Avoid business-rule implementations.

Do not put large reusable UI blocks inside route/page files.

Examples:
```text
CommodityTable
SaleForm
PurchaseForm
ReceiptPrintView
StockLevelBadge
CustomerBalanceCard
```

Do not create abstractions prematurely.

---

## Error Handling

Do not expose stack traces, SQL errors, password hashes, database paths, or internal exception details to the frontend/user.

Convert expected application failures into clear user-facing errors.

Unexpected failures should be logged appropriately without exposing sensitive implementation details.

---

## Secrets & Sensitive Data

Never expose password hashes, authentication secrets, database file paths, or internal exception details to the React frontend.

The SQLite database contains business and financial data and must be treated as sensitive.

---

## Backups

The SQLite database is a local file with no built-in redundancy.

Database-related features must not interfere with the ability to make a simple scheduled file-copy backup.

Do not introduce unnecessary database locking, fragmentation, or additional database files.

---

## Feature Implementation

When building a feature:

1. Read this file first.
2. Identify the files that need to change.
3. Keep changes focused.
4. Do not rewrite unrelated code.
5. Follow existing patterns.
6. Keep business rules in C#.
7. Keep database access behind Infrastructure/Application boundaries.
8. Use the typed WebView bridge for frontend communication.
9. Make sure the feature works end to end.
10. If it affects stock, verify the stock movement is written.
11. If it affects money, verify total/paid/balance behavior.
12. Verify authorization for each affected operation.
13. Fix build, type, and test errors before finishing.

---

## Testing Priorities

At minimum test:
- Authentication
- Role authorization
- Commodity CRUD
- Sale creation
- Purchase creation
- Partial payments
- Balance calculations
- Stock IN
- Stock OUT
- Stock adjustments
- Stock ledger entries
- Historical transaction prices
- Transaction rollback/failure behavior
- User permissions

---

## Documentation

Keep these synchronized with the implementation:

```text
docs/PRODUCT.md
docs/SCREENS.md
docs/ROLES-AND-PERMISSIONS.md
docs/CRUD.md
docs/BUSINESS-RULES.md
docs/FLOWS.md
docs/DESIGN-SYSTEM.md
docs/DATA-MODEL.md
docs/ARCHITECTURE.md
docs/DEPENDENCIES.md
docs/OPEN-DECISIONS.md
```

If implementation changes an established business rule or architecture, update the relevant documentation.

---

## Communication

Be concise.

When completing a feature, explain:
- What changed
- Which important files changed
- How the feature works
- How it was tested
- Any unresolved decision or limitation

---

## Final Reminder

Before every feature:
- Read this file.
- Follow it strictly.
- Keep the implementation simple.
- Preserve established business rules.
- Keep C# as the authoritative business/application layer.
- Keep SQLite behind the C# data-access layer.
- Never let React access SQLite directly.
- Use the typed WebView bridge.
- Store base-unit quantities in kg.
- Store actual prices on transactions.
- Preserve historical transaction values.
- Record stock movements for every stock-affecting operation.
- Enforce permissions in C#.
- Replicate provided UI designs exactly when a reference is supplied.
