# Masolo Agro Commodities Manager

Project documentation for the offline agricultural commodities management application.

## Product summary

Masolo Agro Commodities Manager is a single-PC, offline business management application for an agricultural commodities business. It manages commodities, purchases, sales, payments, stock movements, and auditing.

The application is a **C#/.NET desktop application with a WebView-based UI**.

## Technology stack

- C# / .NET 10
- WebView2
- React + TypeScript
- Vite
- shadcn/ui
- Tailwind CSS
- SQLite
- Entity Framework Core
- Zod for frontend validation
- Local authentication
- Native/local printing

See [DEPENDENCIES.md](DEPENDENCIES.md) for the proposed package list.

## Core principles

- Offline-first; SQLite is the source of truth.
- Single-business / single-PC implementation.
- Tables are the primary information surface.
- Financial and stock history must be preserved accurately.
- Posted transactions should be corrected through controlled operations rather than destructive editing.
- Permissions must be enforced in the C# application layer.
- Keep the application simple and portable.
- Avoid unnecessary cloud services, external authentication, cloud sync, and multi-tenancy.

## Core screens

1. Login
2. Dashboard
3. Commodities
4. Sales
5. Purchases
6. Stock Ledger

Supporting screens implied by the requirements:

- Reports
- Users / User Management
- Account/Profile
- Settings

Reports and User Management are explicitly implied by the role requirements. Account/Profile and Settings are supporting recommendations rather than confirmed requirements.

## Documentation

- [Product](PRODUCT.md)
- [Screens](SCREENS.md)
- [User Roles & Permissions](ROLES-AND-PERMISSIONS.md)
- [CRUD Matrix](CRUD.md)
- [Business Rules](BUSINESS-RULES.md)
- [User Flows](FLOWS.md)
- [Design System](DESIGN-SYSTEM.md)
- [Data Model](DATA-MODEL.md)
- [Architecture](ARCHITECTURE.md)
- [Dependencies](DEPENDENCIES.md)
- [Open Decisions](OPEN-DECISIONS.md)
