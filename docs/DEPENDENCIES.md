# Dependencies (as installed)

Exact versions pinned at foundation setup. Frontend installs via pnpm
(`frontend/pnpm-lock.yaml`); .NET restores via NuGet.

## Toolchain

| Tool | Version |
| --- | --- |
| .NET SDK | 10.0.401 |
| .NET runtime / target | .NET 10 (`net10.0`, App uses `net10.0-windows`) |
| Node.js | 24.20.0 |
| pnpm | 10.11.0 |
| Git | 2.55.0 |
| WebView2 Runtime (machine) | 153.0.4234.48 |

## NuGet (C#)

| Package | Version | In | Why |
| --- | --- | --- | --- |
| `Microsoft.Web.WebView2` | 1.0.4191.47 | App | Embedded browser control (WPF) |
| `Microsoft.EntityFrameworkCore.Sqlite` | 10.0.12 | Infrastructure | SQLite provider, the only database access |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.12 | Infrastructure | Migrations and design time tooling |
| `Microsoft.Extensions.Hosting` | 10.0.12 | App | Generic Host: DI, configuration, logging, lifecycle |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 10.0.12 | Application | `IServiceCollection` contract for `AddApplication` without the full hosting stack |
| `MSTest` | 4.0.2 | Tests | Microsoft supported test framework |

Password hashing uses PBKDF2 from the .NET base library
(`Rfc2898DeriveBytes`), no extra package. No cloud, SaaS, or second ORM
packages. New libraries need approval per AGENTS.md.

## npm (frontend, pnpm)

| Package | Version | Kind | Why |
| --- | --- | --- | --- |
| `react` / `react-dom` | 19.3.0 | dep | UI |
| `zod` | 4.6.5 | dep | Frontend form/input validation (C# revalidates authoritatively) |
| `tailwindcss` / `@tailwindcss/vite` | 4.3.3 | dep/dev | Styling via the Vite plugin |
| `clsx` | 2.1.1 | dep | Conditional class names (shadcn helper) |
| `tailwind-merge` | 3.7.0 | dep | Class conflict resolution (shadcn helper) |
| `class-variance-authority` | 0.7.1 | dep | Component variants (shadcn helper) |
| `lucide-react` | 1.48.0 | dep | Icons |
| `vite` | 8.3.1 | dev | Build tooling and dev server on port 5173 |
| `@vitejs/plugin-react` | 6.1.1 | dev | React support for Vite |
| `typescript` | 5.9.3 | dev | Strict typechecking (see note) |
| `@types/react` / `@types/react-dom` | 19.3.0 | dev | React types |
| `@types/node` | 26.6.2 | dev | Node types for `vite.config.ts` |
| `eslint` | 10.11.0 | dev | Linting |
| `typescript-eslint` | 8.70.1 | dev | TS rules for ESLint |
| `eslint-plugin-react-hooks` | 7.1.1 | dev | Hooks rules |
| `eslint-plugin-react-refresh` | 0.5.7 | dev | Refresh rules |
| `@eslint/js` | 10.0.1 | dev | Core recommended rules |
| `globals` | 17.12.0 | dev | Browser globals for ESLint |

Note: TypeScript stays at 5.9.3 because `typescript-eslint` 8.70.1 only
supports TS below 6.1; TS 7 breaks the lint resolve. Revisit when the
ESLint TS tooling supports newer TypeScript. No global state library, no
Electron/Tauri, no Next.js.
