# CRUD Matrix

## Entity-level matrix

| Entity / Area | Owner | Manager | Cashier | Notes |
|---|---|---|---|---|
| Users | CRUD | CRUD | — | User management |
| Commodities | CRUD | CRUD | R | Protect historical references |
| Sales | CRUD* | CRUD* | CR | Posted sales use controlled correction/reversal |
| Purchases | CRUD* | CRUD* | — | Posted purchases use controlled correction/reversal |
| Stock Ledger | R | R | R | Ledger itself is not directly edited |
| Stock Adjustments | CR | CR | — | Controlled operation creates ledger entry |
| Reports | R | R | — | Generated/read-only |
| Dashboard | R | R | R | Read-only aggregation |
| Account | RU | RU | RU | Supporting functionality |

## Legend

- **C** — Create
- **R** — Read
- **U** — Update
- **D** — Delete
- **CR** — Create + Read
- **RU** — Read + Update
- **—** — No access
- **\*** — Controlled correction/reversal rather than unrestricted destructive CRUD

## CRUD behavior by area

### Commodities

- Create new commodity.
- Read/list commodity records.
- Update commodity information.
- Delete only when doing so does not compromise historical transaction integrity.

### Sales

- Create sale.
- Read sales and sale details.
- Record partial payments.
- Print receipt.
- Correct/reverse posted transactions through controlled workflow.
- Avoid destructive deletion of posted stock-affecting history.

### Purchases

- Create purchase.
- Read purchases and purchase details.
- Record payment and balance.
- Correct/reverse posted transactions through controlled workflow.
- Avoid destructive deletion of posted stock-affecting history.

### Stock Ledger

- Read/filter historical movements.
- Do not edit ledger rows directly.
- Create stock changes through a controlled stock-adjustment operation.

### Users

- Create users.
- Read/list users.
- Update users.
- Delete/deactivate behavior remains an implementation decision because the source does not define the exact lifecycle.

### Reports

Reports are generated/read-only views over stored business data.
