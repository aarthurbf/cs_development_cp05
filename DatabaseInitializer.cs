using Microsoft.Data.Sqlite;

namespace CadastroProdutos;

public static class DatabaseInitializer
{
    public static void Initialize(string connectionString)
    {
        // Garante que o diretório database/ exista antes de criar o arquivo .db
        var builder = new SqliteConnectionStringBuilder(connectionString);
        var dbDir = Path.GetDirectoryName(builder.DataSource);
        if (!string.IsNullOrEmpty(dbDir))
            Directory.CreateDirectory(dbDir);

        var scriptPath = Path.Combine(AppContext.BaseDirectory, "Scripts", "create_table.sql");
        var script = File.ReadAllText(scriptPath);

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = new SqliteCommand(script, connection);
        command.ExecuteNonQuery();
    }
}
