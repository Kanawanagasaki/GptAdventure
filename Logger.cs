namespace GptAdventure;

public static class Logger
{
    private const string _filename = "log.txt";
    private static Stream _fileStream;
    private static StreamWriter _writer;

    static Logger()
    {
        _fileStream = File.Open(_filename, FileMode.Create, FileAccess.Write, FileShare.Read);
        _writer = new StreamWriter(_fileStream);
        WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
    }

    public static void WriteLine(object? obj)
    {
        _writer.WriteLine($"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] " + (obj?.ToString() ?? "NULL"));
        _writer.Flush();
    }
}
