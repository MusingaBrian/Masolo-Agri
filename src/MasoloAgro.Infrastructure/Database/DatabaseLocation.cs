namespace MasoloAgro.Infrastructure.Database;

/// <summary>
/// Locates the local SQLite database file. The database lives on this PC
/// only, so a plain file copy is a valid backup.
/// </summary>
public static class DatabaseLocation
{
    private const string DatabaseFileName = "masoloagro.db";

    public static string GetDefaultConnectionString()
    {
        Directory.CreateDirectory(GetDirectory());
        return $"Data Source={GetDatabasePath()}";
    }

    public static string GetDatabasePath()
        => Path.Combine(GetDirectory(), DatabaseFileName);

    private static string GetDirectory()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MasoloAgro");
}
