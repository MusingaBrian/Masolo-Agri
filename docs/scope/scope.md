# Scope

You track build phases here. Each row stays coarse. Detail lives in the linked spec.

## At a glance

1. Initial database schema, status done, spec `0001`.

## Initial database schema, status done

Intent: You ship one offline schema for users plus stock plus deals plus money plus audit, so later trade features need no rework.

Spec: [0001](../specs/0001-initial-database-schema/index.md)

Done when: `AC-1` through `AC-9` in the spec all hold on a temp file plus tests pass.

1. [x] Design it (spec)
2. [x] Build it: /develop initial database schema
    1. [x] Domain plus configs plus context, covers `AC-1` plus `AC-6` plus `AC-8`
    2. [x] Migration plus Owner seed plus clean creation, covers `AC-1` plus `AC-8`
    3. [x] Tests for money plus kg plus guards plus reversal plus rollback, covers `AC-2` plus `AC-3` plus `AC-6` plus `AC-7` plus `AC-9`

    Code in `src/MasoloAgro.Domain/Entities/` plus `src/MasoloAgro.Domain/Enums/` plus `src/MasoloAgro.Infrastructure/Database/` plus `src/MasoloAgro.Tests/SchemaModelTests.cs` plus `src/MasoloAgro.Tests/SchemaGuardTests.cs`.
3. [x] Verify it: /check verify initial database schema
4. [x] Test it: /test initial database schema
