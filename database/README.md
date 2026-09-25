# Database

The SQLite database file lives outside this repo on the business PC at
`%LocalAppData%/MasoloAgro/masoloagro.db` (see `DatabaseLocation` in
`MasoloAgro.Infrastructure`). Never commit database files.

Schema changes go through EF Core migrations from the Infrastructure project:

```powershell
dotnet ef migrations add <Name> --project src/MasoloAgro.Infrastructure --startup-project src/MasoloAgro.App
dotnet ef database update --project src/MasoloAgro.Infrastructure --startup-project src/MasoloAgro.App
```

Migrations generate C# code inside the Infrastructure project. The
`database/migrations/` folder is reserved for any hand written operational
notes about the local database file, not for schema SQL.
