# Open Decisions

This file records areas where the available requirements do not define a final behavior.

## Receipt paper format

Not yet decided:

- Thermal receipt
- A4
- Both

The receipt layout should remain independent of the normal UI regardless of the final format.

## Payment settlement workflow

The requirements define:

- Total
- Amount paid
- Balance

They do not fully define how a later payment against an existing balance is recorded.

Possible future design needs:

- Payment history
- Add-payment action
- Payment date
- Payment method
- Receipt for subsequent payment

Do not implement these as confirmed requirements until decided.

## User deletion lifecycle

User management is required for Owner and Manager, but the exact behavior is not specified.

Decide between:

- Hard delete
- Deactivation
- Soft delete
- Another lifecycle

For auditability, a non-destructive lifecycle may be considered, but this is a design decision rather than a confirmed rule.

## Report catalog

Reports are required for Owner and Manager, but the complete report list and filters are not defined.

Previously identified candidate areas:

- Sales
- Purchases
- Stock
- Balances

Treat these as planned areas until confirmed.

## Stock adjustment fields

The requirements establish controlled stock adjustments but do not define the complete form.

Potential fields requiring confirmation:

- Adjustment quantity
- Direction
- Reason
- Notes
- Date/time
- User
- Approval requirement

## Commodity deletion

The requirements imply historical transactions must remain valid.

The exact UX for deleting a referenced commodity should be decided, such as:

- Prevent deletion
- Archive/deactivate commodity
- Allow deletion only when unused

## Dashboard metrics

The dashboard is confirmed as a core screen, but the complete metric set is not explicitly defined.

Do not invent final metrics without confirmation.

## Payment methods

No definitive payment-method model was specified.

Avoid assuming cash/mobile money/bank/etc. until confirmed.

## Customer/supplier master records

Sales and purchases are confirmed, but the available requirements do not establish whether customers and suppliers require separate master-data CRUD screens.

Do not add them as confirmed entities without a product decision.
