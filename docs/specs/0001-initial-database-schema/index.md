# 0001. Initial database schema

**Date**: `2026-09-25`
**Status**: Accepted

## Summary

You get a full offline store for your trade in one versioned schema. It keeps users plus stock plus deals plus money plus audit in SQLite through EF Core (a mapper that lets C# talk to the database in plain objects). You may build from it directly, and you may grow later work on top with no rework.

## Requirements

**User stories**:
1. As an Owner, you want one schema that holds users plus commodities plus deals plus stock moves so you may open the shop with no rework.
2. As a Cashier, you want sales with many lines plus partial pay so you may serve mixed baskets fast.
3. As a Manager, you want purchases plus adjustments with reasons so you may explain every stock change.

**Acceptance criteria**:
1. `AC-1`: One EF Core migration creates all twelve tables with keys plus links and applies clean to a temp file.
2. `AC-2`: Money uses decimal with whole shillings by default, never float, and headers store total plus paid plus balance.
3. `AC-3`: Quantities store in kg as decimal, and each line stores its own deal time unit price plus line total.
4. `AC-4`: A sale may hold many lines, a purchase may hold many lines, and each may link to many movements and many payments.
5. `AC-5`: Customers and suppliers exist as linked masters with optional links plus a name snapshot on each deal, so quick cash deals still pass.
6. `AC-6`: Guards hold in the model, with unique username plus token plus commodity name plus deal refs, with quantity above zero, with paid within zero to total, with safe delete rules.
7. `AC-7`: Posted deals stay in place, and a fix uses status plus reversing movement, never hard delete.
8. `AC-8`: Local sign in works from stored users with hashed passwords plus roles plus stored sessions with expiry.
9. `AC-9`: Model validation plus critical tests pass for creation plus guards plus math plus rollback.

## Decision

**Chosen option**: Option 2: Full traceable schema

You may build the twelve table schema with EF Core plus SQLite, with fluent configs (small rule classes that shape each table), with one migration, with a runtime seed for the first Owner, and with tests that prove creation plus guards plus math.

## Feature design

**Data model sketch**:
1. User. Id `Guid` key (a random id that stays unique). Username unique required. PasswordHash required (scrambled password, never plain). Role int required with `Owner` `Manager` `Cashier`. IsActive bool. CreatedAt `DateTime` plus UpdatedAt `DateTime`. One user has many sessions plus deals plus moves.
2. UserSession. Id `Guid` key. UserId required link to User, with one to many. Token unique required (a secret string that proves sign in). CreatedAt plus ExpiresAt plus RevokedAt optional. Index on Token plus UserId.
3. Commodity. Id `Guid` key. Name unique required. CurrentPrice decimal `18,2` (exact money with two places). StockKg decimal `18,3` (exact weight with three places). ReorderLevel decimal `18,3`. IsActive bool. CreatedAt plus UpdatedAt.
4. Customer. Id `Guid` key. Name required. Phone optional. Notes optional. IsActive bool. CreatedAt plus UpdatedAt. One customer has many sales.
5. Supplier. Id `Guid` key. Name required. Phone optional. Notes optional. IsActive bool. CreatedAt plus UpdatedAt. One supplier has many purchases.
6. Sale. Id `Guid` key. RefNumber unique required (human receipt code). SaleDate required. CustomerId optional link to Customer, with many to one. CustomerName required snapshot (buyer name copied at deal time). Total decimal `18,2` plus AmountPaid decimal `18,2` plus BalanceDue decimal `18,2`. Status int with `Posted` `Reversed`. CreatedBy link to User. CreatedAt plus ReversedAt optional plus ReversalReason optional.
7. SaleLine. Id `Guid` key. SaleId required link to Sale, with one to many, with fall with parent. CommodityId required link to Commodity, with many to one, with block on delete. QuantityKg decimal `18,3` above zero. UnitPrice decimal `18,2` (price at deal time). LineTotal decimal `18,2` (quantity times price).
8. Purchase. Id `Guid` key. RefNumber unique required. PurchaseDate required. SupplierId optional link to Supplier, with many to one. SupplierName required snapshot. Total plus AmountPaid plus BalanceDue decimal `18,2`. Status int with `Posted` `Reversed`. CreatedBy link to User. CreatedAt plus ReversedAt optional plus ReversalReason optional.
9. PurchaseLine. Id `Guid` key. PurchaseId required link to Purchase, with one to many, with fall with parent. CommodityId required link to Commodity, with many to one, with block on delete. QuantityKg decimal `18,3` above zero. UnitPrice decimal `18,2`. LineTotal decimal `18,2`.
10. StockAdjustment. Id `Guid` key. CommodityId required link. QuantityChangeKg decimal `18,3` (positive for gain, negative for loss, never zero). Reason required. CreatedBy link to User. AdjustmentDate required plus CreatedAt.
11. StockMovement. Id `Guid` key. CommodityId required link. MovementType int with `In` `Out` `Adjustment` (direction of move). QuantityKg decimal `18,3` above zero (always positive, direction lives in type). MovementDate required. CreatedBy link to User. Reason optional. SaleId optional plus PurchaseId optional plus StockAdjustmentId optional, with only one set per row.
12. Payment. Id `Guid` key. SaleId optional plus PurchaseId optional, with exactly one set per row. Amount decimal `18,2` above zero. PaymentDate required. ReceivedBy link to User. Method optional free text (you keep it open, no fixed list yet). Notes optional.

**State transitions**:
1. Sale: `Posted` to `Reversed`, with reversing movement plus reason. No delete when posted.
2. Purchase: `Posted` to `Reversed`, with reversing movement plus reason. No delete when posted.
3. Commodity: active to inactive, never delete with history.
4. User: active to disabled, never delete the last Owner.
5. Payment: posted and immutable, fix with new offsetting payment.

**API surface**:
1. You may expose `DbSet` for all twelve tables through `AppDbContext`, with no direct use from the UI.
2. You may keep one fluent config class per table in `src/MasoloAgro.Infrastructure/Database/Configurations/`, with precision plus required plus unique plus delete rules.
3. You may keep `AppDbContextFactory` for design time creation, plus `DatabaseLocation` for the local file path.
4. You may add `DbSeeder.EnsureOwnerAsync` that creates the first Owner when the users table is empty.
5. You may add one migration `InitialSchema` that builds the full target in one pass.

**Value sourcing**:
1. Sale Total: source is sum of `SaleLine.LineTotal` at write time, stored on `Sale.Total`.
2. Sale LineTotal: source is `QuantityKg` times `UnitPrice`, stored on `SaleLine.LineTotal`.
3. Sale BalanceDue: source is `Total` minus `AmountPaid`, stored on `Sale.BalanceDue`.
4. Sale AmountPaid: source is sum of linked `Payment.Amount` at write time, stored for fast reads.
5. Purchase Total plus paid plus balance: source mirrors sale logic from `PurchaseLine` plus linked payments.
6. CustomerName plus SupplierName: source is master name copied at deal time, stored as snapshot.
7. StockMovement QuantityKg: source is line quantity for deals, change size for adjustments, always positive with type giving direction.
8. CurrentPrice on Commodity: source is latest maintenance edit, never used to rebuild old lines.
9. Session Token: source is secure random string created at sign in, stored with expiry.
10. RefNumber: source is date plus sequence created in app code before insert, stored unique.

**Key invariants**:
1. Stock stores in kg only, with display change to bags or tonnes outside the store.
2. Lines keep deal time price, and current price edits never touch old lines.
3. Paid stays within zero to total, and balance equals total minus paid.
4. Movements never edit in place, and fixes add new rows.
5. Deletes block for commodity plus customer plus supplier with history, while lines fall with the parent deal.
6. Payments link to exactly one parent, and amounts stay above zero.

**Security model**:
You store `CreatedBy` plus role on each write, and you enforce Owner plus Manager plus Cashier rules in app services later. Cashier scope stays to sales plus stock reads plus dashboard, with no commodity edits plus no reports plus no user care plus no purchases plus no adjustments. Passwords stay hashed, tokens expire, and no secret leaves to the UI.

**Configuration required**:
You need no new env values. You reuse the local file path from `DatabaseLocation` in `src/MasoloAgro.Infrastructure/Database/DatabaseLocation.cs`.

**Critical test scenarios**:
1. Happy path: you create a commodity plus a purchase with two lines plus a sale with two lines plus one partial payment, and stock plus totals plus balance all match, verifies `AC-1` plus `AC-2` plus `AC-3` plus `AC-4`.
2. Failure case: you post a sale with too much paid plus a line with zero quantity plus a delete of a used commodity, and each fails with no partial save, verifies `AC-6`.
3. Reversal case: you reverse a posted sale and a new movement restores stock while the old rows stay, verifies `AC-7`.
4. Auth case: you seed the first Owner plus sign in plus expire a session, and hash verify passes while wrong password fails, verifies `AC-8`.
5. Rollback case: you fail a multi row write mid way and no header plus no lines plus no movements remain, verifies `AC-9`.

## Build plan

1. You may create the twelve Domain entities with plain properties plus enums for role plus status plus movement type, satisfies `AC-1` plus `AC-8`.
2. You may add twelve fluent config classes with precision plus uniques plus links plus delete rules, satisfies `AC-6`.
3. You may wire `AppDbContext` with all sets plus model apply from assembly plus factory reuse, satisfies `AC-1`.
4. You may create and apply the `InitialSchema` migration to a temp file and prove clean creation, satisfies `AC-1`.
5. You may add the runtime Owner seed with hash plus active check, satisfies `AC-8`.
6. You may add model tests for money exactness plus kg store plus deal time prices plus totals math, satisfies `AC-2` plus `AC-3`.
7. You may add guard tests for uniques plus quantity plus paid range plus safe deletes plus reversal plus rollback, satisfies `AC-6` plus `AC-7` plus `AC-9`.

## Consequences

**Positive**:
1. You hold one base that all next features may reuse with no rework.
2. You keep audit plus money proof from day one, which helps trust and reports.
3. You keep SQLite file copy backup workable, with one file and no extra server.

**Negative plus tradeoffs**:
1. You carry twelve tables before any screen ships, which asks for careful review now.
2. You store totals as well as lines, so app code must keep them in sync at write time.
3. You defer strict cashless rules plus paper format plus report list, so those need later specs.

**Neutral**:
1. You keep method as free text, so a fixed pay method list may arrive later with a small field change.

## Follow up

1. You may spec auth services with sign in plus session plus role checks over this schema when you start feature one.
2. You may spec sale plus purchase write flows with transactions plus movement creation plus receipt data when you reach trade features.
3. You may confirm receipt paper size plus report list plus pay method list before you build those screens.
4. You may enroll a scope row that points to this spec if you start using `docs/scope/` to track build phases.
