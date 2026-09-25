# Architecture (as built)

Single PC offline desktop app. React frontend in an embedded browser,
C# as the authoritative application layer, SQLite as the only database.

## Projects

```text
MasoloAgro.App            WPF (.NET 10, Windows) + WebView2 host
MasoloAgro.Application    use cases, business rules, ports (interfaces)
MasoloAgro.Domain         entities, enums, value objects, exceptions
MasoloAgro.Infrastructure EF Core, SQLite, security primitives
MasoloAgro.Tests          MSTest
frontend/                 React + TypeScript + Vite (pnpm)
```

Reference direction:

```text
App -> Application, Infrastructure
Application -> Domain
Infrastructure -> Domain, Application (see note)
Tests -> Domain, Application, Infrastructure
Domain -> nothing
```

Note: Infrastructure references Application for one reason only, so that
adapters can implement application defined ports (dependency inversion,
e.g. `Pbkdf2PasswordHasher` implements `IPasswordHasher`). Domain stays
free of every dependency. Frontend references no C# project; it talks to
C# only through the bridge.

## Runtime flow

```text
React UI action
  ↓  typed bridge call { id, request: { operation, payload } }
WebViewBridge.DispatchJsonAsync
  ↓  registered C# handler for that exact operation name
Application service (validation + authorization + business rules)
  ↓  EF Core inside a transaction where stock or money moves
SQLite (%LocalAppData%/MasoloAgro/masoloagro.db)
  ↓  { id, response: { ok, payload, error } } posted back to the WebView
```

The DbContext is never exposed to JavaScript. Failures cross the bridge as
short user facing messages; stack traces, SQL, and secrets stay in C# logs.

## WebView hosting

`MainWindow` (WPF) hosts the WebView2 control. `WebViewHost` loads the
Vite dev server (`http://localhost:5173`) in DEBUG builds and the bundled
`wwwroot/index.html` in Release builds. The frontend build outputs
directly to `src/MasoloAgro.App/wwwroot/` (see `frontend/vite.config.ts`).

## Authentication foundation

Local users in SQLite, PBKDF2 password hashes (`Pbkdf2PasswordHasher`),
roles `Owner / Manager / Cashier` (`UserRole`). No cloud provider.
Login/session behavior arrives with the auth feature.

## Development commands

```powershell
pnpm --dir frontend install    # frontend dependencies
pnpm --dir frontend dev        # Vite dev server (auto opened by the app in DEBUG)
pnpm --dir frontend typecheck  # strict TypeScript check
pnpm --dir frontend lint       # ESLint
pnpm --dir frontend build      # typecheck + production bundle into wwwroot

dotnet restore MasoloAgro.sln  # NuGet restore
dotnet build MasoloAgro.sln    # build all projects
dotnet test MasoloAgro.sln     # run the test suite

powershell -File scripts/build-frontend.ps1  # install + bundle frontend
powershell -File scripts/publish.ps1         # bundle + publish win-x64 single file to publish/
```

## Production build

`scripts/publish.ps1` bundles the frontend, then publishes
`MasoloAgro.App` as a framework dependent win-x64 single file into
`publish/`. The .NET 10 runtime must be installed on the business PC.
