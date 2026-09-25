# Product

## Name

Masolo Agro Commodities Manager

## Purpose

Manage the day-to-day operations of an agricultural commodities business from a single offline computer.

## Primary operational areas

- Commodity management
- Purchasing
- Sales
- Payments and balances
- Stock tracking
- Stock adjustments
- Stock audit trail
- Reporting
- Local user access control
- Printable sales receipts

## Operating model

- Single business
- Single-PC / offline
- C# / .NET desktop host
- WebView-based frontend
- Local SQLite database
- Entity Framework Core for data access
- Local authentication
- No cloud synchronization
- No external authentication
- Database is the source of truth

## Primary users

### Owner

Full access to the application, including reports and user management.

### Manager

Full operational access, including reports and user management.

### Cashier

Operational sales access and stock visibility.

Cashier restrictions:

- Cannot edit commodities.
- Cannot view reports.
- Cannot manage users.
- Cannot perform stock adjustments.
- Cannot record purchases.

## Product philosophy

The interface is intended for staff who use the system for hours at a time. The UI should therefore favor:

- Scanability
- Compact information density
- Predictable forms
- Clear numeric formatting
- Minimal visual decoration
- Reliable transaction history
- Explicit status indicators
