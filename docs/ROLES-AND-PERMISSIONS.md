# User Roles & Permissions

## Roles

| Role | Description |
|---|---|
| Owner | Full access, including reports and user management |
| Manager | Full operational access, including reports and user management |
| Cashier | Records sales and views stock; restricted from administration and reporting |

## Permission matrix

| Area | Owner | Manager | Cashier |
|---|---:|---:|---:|
| Login | ✓ | ✓ | ✓ |
| Dashboard | Read | Read | Read |
| Commodities | CRUD | CRUD | Read |
| Sales | CRUD* | CRUD* | Create + Read |
| Purchases | CRUD* | CRUD* | — |
| Stock Ledger | Read | Read | Read |
| Stock Adjustments | Create + Read | Create + Read | — |
| Reports | Read | Read | — |
| Users | CRUD | CRUD | — |
| Account | Read + Update | Read + Update | Read + Update |

## Important permission rule

Permissions must be enforced on the server/application data-access boundary. Hiding navigation items or buttons is not sufficient.

## Posted transaction rule

The `CRUD*` notation does not mean that posted financial/stock transactions should be freely deleted.

For sales and purchases that have already affected stock or financial history, use controlled correction/reversal behavior so that historical records and the stock audit trail remain reliable.

## Cashier boundaries

A Cashier:

- Can record sales.
- Can view stock.
- Can view relevant sales records.
- Cannot edit commodities.
- Cannot record purchases.
- Cannot perform stock adjustments.
- Cannot view reports.
- Cannot manage users.
