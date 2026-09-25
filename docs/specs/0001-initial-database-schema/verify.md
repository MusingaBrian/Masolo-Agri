# Verify: initial database schema · spec 0001 · updated 2026-09-25
_Steps derived from spec 0001 acceptance criteria. `/check verify` runs these; `/test` can later lock the durable ones._
## Manual
- [x] Open the fresh temp database file in your SQLite viewer of choice, expect all twelve tables present with keys plus links → AC-1
## Commands
- [x] `dotnet build src/MasoloAgro.App -c Release` → 0 warnings, 0 errors → AC-1
- [x] `dotnet test src/MasoloAgro.Tests -c Release` → 21 of 21 passed → AC-9
- [x] `dotnet-ef database update` to a temp file → clean apply, no errors → AC-1
- [x] Happy path test (purchase with two lines plus sale with two lines plus partial pay) → totals plus balance match → AC-2, AC-3, AC-4
- [x] Guard tests (overpaid sale, zero quantity line, used commodity delete) → each fails with no partial save → AC-6
- [x] Reversal test (reverse a posted sale) → old rows stay plus restoring movement added → AC-7
- [x] Seed test (first Owner plus hash plus expiry) → pass, wrong password fails → AC-8
- [x] Rollback test (fail a multi row write midway) → no header plus no lines remain → AC-9
## Value source checks
- [x] Sale Total from sum of `SaleLine.LineTotal` at write time → HappyPath expects 25000
- [x] Sale LineTotal from quantity times price → Money test expects 5000
- [x] Sale BalanceDue from total less paid → Money test expects balance math holds
- [x] Sale AmountPaid from sum of linked payments → HappyPath expects 10000
- [x] Purchase total plus paid plus balance mirrors sale logic → HappyPath expects 350000 plus 200000 plus 150000
- [x] Customer plus supplier names copied at deal time → HappyPath uses walk in names
- [x] Movement quantity positive with direction in type → HappyPath expects 4 moves
- [x] CurrentPrice edit keeps old lines fixed → DealTimePrice test keeps 2800 after move to 3500
- [x] Session token with expiry → Session test reads expired token row
- [x] RefNumber date plus sequence stored unique → Unique sale ref test blocks doubles
## Acceptance-criteria coverage
- AC-1 · covered by build plus migration plus table steps · AC-2 · covered by happy path plus money steps · AC-3 · covered by happy path plus deal time steps · AC-4 · covered by happy path step · AC-5 · covered by snapshot plus optional link shape in happy path · AC-6 · covered by guard steps · AC-7 · covered by reversal step · AC-8 · covered by seed step · AC-9 · covered by test plus rollback steps
