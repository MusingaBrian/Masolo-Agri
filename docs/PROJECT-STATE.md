# Project State (persistent session handoff)

Last updated: 2026-09-25. Update this file at the end of every session.
Do not claim work here unless it was implemented and verified.

## Current phase

Foundation plus initial database schema complete, verified, tested, and
marked done (spec 0001 Accepted). All scope boxes show done. Suggested
next work is auth plus users on top of this schema.

## Completed work

- Environment verified: .NET SDK 10.0.401, Node 24.20.0, pnpm 10.11.0,
  Git 2.55.0, WebView2 Runtime 153.0.4234.48 on the machine.
- `MasoloAgro.sln` (classic format) with 5 projects: App (WPF,
  `net10.0-windows`), Domain, Application, Infrastructure, Tests
  (`net10.0`). Project references wired per the dependency direction.
- NuGet installed: WebView2 1.0.4191.47, EFCore.Sqlite 10.0.12,
  EFCore.Design 10.0.12, Extensions.Hosting 10.0.12,
  DI.Abstractions 10.0.12, MSTest 4.0.2.
- C# foundation: `UserRole` enum, `IPasswordHasher` port,
  `Pbkdf2PasswordHasher`, `AppDbContext` + design-time factory +
  `DatabaseLocation` (`%LocalAppData%/MasoloAgro/masoloagro.db`),
  `AddApplication` / `AddInfrastructure` composition, Generic Host in
  `App.xaml.cs`, `MainWindow` hosting WebView2, typed bridge
  (`BridgeOperations`, `BridgeRequest`/`BridgeResponse`, `WebViewBridge`
  with `{id, request}` / `{id, response}` envelopes), `ReceiptPrinter`
  seam (throws until the sales feature arrives).
- Frontend (`frontend/`, pnpm): React 19.3.0, Vite 8.3.1, Tailwind 4.3.3,
  shadcn infra (`cn` helper, `components.json`, theme tokens matching the
  design system), strict TS, ESLint, bridge mirror (`lib/bridge/`), UGX
  and kg formatting helpers, placeholder `App.tsx` only.
- Quality gates: `Directory.Build.props` (nullable, implicit usings,
  warnings-as-errors in Release), `.editorconfig`, `.gitignore`
  (verified: `bin/`, `node_modules/`, `wwwroot/`, `*.db` ignored).
- `scripts/build-frontend.ps1` and `scripts/publish.ps1`
  (framework-dependent win-x64 single file into `publish/`).
- Docs rewritten to as-built state: `docs/ARCHITECTURE.md`,
  `docs/DEPENDENCIES.md`, `database/README.md`.
- Git initialized, committed, and pushed to
  https://github.com/MusingaBrian/Masolo-Agri (`main` tracks
  `origin/main`). History: `cb725c7` (remote template README) +
  foundation commit on top via rebase; README conflict resolved in favor
  of the project README. Commit identity: MusingaBrian with the GitHub
  noreply email.
- 2 MSTest smoke tests pass: hasher round-trip, SQLite in-memory connect.
- Spec 0001 built: twelve Domain entities plus SaleStatus plus
  PurchaseStatus plus StockMovementType, twelve fluent configs, AppDbContext
  with twelve sets plus assembly scan, InitialSchema migration, runtime
  DbSeeder.EnsureOwnerAsync for the first Owner.
- 19 new MSTest tests pass (21 of 21 with smoke): money exactness, kg store,
  deal time prices, totals math, happy path purchase plus sale plus partial
  pay, uniques, quantity plus paid range, safe deletes, reversal, rollback,
  seed plus sign in, session expiry.
- Schema guard note: SQLite keeps decimal values as text, so every numeric
  guard compares after CAST to REAL. The note lives in code in
  src/MasoloAgro.Infrastructure/Database/AppDbContext.cs.
- Spec 0001 promoted to a folder with index plus rationale plus verify files.

## Work in progress

Nothing. The schema feature is done and closed out.

## Remaining tasks

1. Start the auth/users feature: sign in plus logout, session over the
   bridge, role enforcement in C#, built on the new User plus UserSession
   tables.

## Technical decisions

- TypeScript pinned at 5.9.3, not latest 7.0.2: typescript-eslint 8.70.1
  only supports TS below 6.1. Revisit when the lint tooling catches up.
- pnpm is the frontend package manager (`pnpm-lock.yaml`); npm lockfile
  removed.
- Classic `.sln` format (SDK defaults to `.slnx`).
- Infrastructure references Application for one reason only: adapters
  implementing application-defined ports (e.g. hasher). Domain is
  dependency-free. Flagged to the user as a possible restructuring point.
- `vite.config.ts` uses `import.meta.dirname` (Vite 8 native config
  loader direction) and builds directly into
  `src/MasoloAgro.App/wwwroot/` (gitignored, generated).
- `database/migrations/` holds operational notes only; real migrations
  live as EF Core code in Infrastructure.

## Important files

- `src/MasoloAgro.App/WebView/` (bridge), `Windows/MainWindow.*`,
  `App.xaml.cs`, `Printing/ReceiptPrinter.cs`
- `src/MasoloAgro.Infrastructure/Database/`, `Security/`
- `src/MasoloAgro.Application/Common/Interfaces/IPasswordHasher.cs`
- `src/MasoloAgro.Domain/Enums/UserRole.cs`
- `src/MasoloAgro.Tests/FoundationSmokeTests.cs`
- `src/MasoloAgro.Domain/Entities/` (twelve entities) plus
  `src/MasoloAgro.Domain/Enums/` (UserRole plus SaleStatus plus
  PurchaseStatus plus StockMovementType)
- `src/MasoloAgro.Infrastructure/Database/` (AppDbContext, twelve configs in
  `Configurations/`, `DbSeeder`, `Migrations/` with InitialSchema)
- `src/MasoloAgro.Tests/SchemaModelTests.cs`,
  `src/MasoloAgro.Tests/SchemaGuardTests.cs`
- `frontend/src/lib/bridge/`, `frontend/src/lib/formatting/`
- `scripts/`, `Directory.Build.props`, `.editorconfig`, `.gitignore`

## Known errors / blockers

- None open. One fixed this session: Release-only `using System.IO`
  missing in `WebViewHost.cs` (DEBUG never compiles that `#else`
  branch). Lesson recorded: always build Release, not just Debug.
- Tool runner note: long `bash` commands with pipes/chains sometimes get
  killed (`Unknown: ChildProcess.kill`); single commands and
  `--reporter=append-only` for pnpm worked reliably.
- Fixed while building spec 0001: six guard tests first failed because
  SQLite keeps decimal values as text (plain compare never fires) and
  because tracked child rows soften deletes client side. Fixed with CAST to
  REAL in configs plus ChangeTracker.Clear with key only stubs in the delete
  tests. Lesson recorded: numeric guards on SQLite need CAST, delete guard
  tests must clear tracked rows first.

## Verification status (last full pass, all green)

- `dotnet build src/MasoloAgro.App -c Release` (0 warnings, 0 errors)
- `dotnet test src/MasoloAgro.Tests -c Release` (32 of 32 passed)
- Fresh `InitialSchema` migration applies clean with `dotnet-ef database
  update` to temp files (two passes: first shape, then CAST guard shape)
- Frontend not touched this session (`pnpm typecheck`, `pnpm lint`,
  `pnpm build` last green at foundation)

## Recommended starting point for the next session

1. Read AGENTS.md and this file.
2. Run `git status` and `git log --oneline -5`.
3. Suggested next: `/check verify initial database schema` (scope box 3),
   then `/test initial database schema` (scope box 4), then start the
   auth/users feature on the new schema.
