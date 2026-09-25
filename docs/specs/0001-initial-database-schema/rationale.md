# 0001 rationale: initial database schema

Decision record for spec 0001. Build content lives in `index.md` beside this file.

## Context

You run one shop on one PC with no internet need. You buy stock in, you sell stock out, you allow partial pay, and you must prove what moved and what money stayed open.

Today the context has tables for code only, with no business tables and no migration. Without a shared schema, each next feature would invent its own fields for price plus quantity plus totals. That would break history plus audit plus reports.

You need one coherent target that future sales plus purchases plus stock plus users can all use. It must keep kg as the base store, keep deal time prices fixed, keep totals plus paid plus balance visible, and keep every stock change traceable.

## Options considered

### Option 1: Lean core only
You keep users plus commodities plus single line deals plus simple movements. Totals live on the header with no payment table and no party masters.

**Pros**:
1. Fewest tables, fastest first pass.

**Cons**:
1. You must reopen files to add lines plus payments plus parties, which breaks receipts and audit.

### Option 2: Full traceable schema
You keep twelve tables with multi line deals plus payment history plus linked party masters plus sessions plus adjustments plus linkable movements. Headers store totals, lines store deal time prices, movements trace to source.

**Pros**:
1. One target that covers your request plus audit plus partial pay with no rework.
2. History stays fixed while current prices may move.

**Cons**:
1. More files and configs up front, which costs a little extra review time.

### Option 3: Derived ledger only
You keep masters strict and derive totals live from lines with no stored balance. Every deal requires a party row.

**Pros**:
1. Totals never drift from lines by shape.

**Cons**:
1. Reads cost more, history may shift if lines change, and quick cash sales slow down.

## Rationale

You asked for full coverage with parties plus payments plus lines plus audit in one pass, and your open notes allow linked masters with later pay. Lean core would force a second schema pass as soon as receipts arrive. Derived ledger would slow daily sales and risk shifting history. Full traceable costs a little more review now, but it lets sales plus purchases plus stock plus users all ship on one base with no rework.
