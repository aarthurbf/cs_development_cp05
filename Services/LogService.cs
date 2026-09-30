namespace CadastroProdutos.Services;

public static class LogService
{
    private static readonly string LogDir = Path.Combine(AppContext.BaseDirectory, "logs");
    private static readonly string LogFile = Path.Combine(LogDir, "app.log");

    public static void Log(string mensagem)
    {
        try
        {
            Directory.CreateDirectory(LogDir);
            var linha = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensagem}";
            File.AppendAllText(LogFile, linha + Environment.NewLine);
        }
        catch
        {
        }
    }
}
