namespace GptAdventure;

public static class AzureConfig
{
    private const string FILENAME = "azure.settings.config";
    private static readonly string[] CONFIGKEYS = new[]
    {
        "APPNAME", "KEY", "REGION", "TTS_ENDPOINT", "VOICES_ENDPOINT", "RECOGNITION_ENDPOINT"
    };

    public static string? AppName => _config.ContainsKey("APPNAME") ? _config["APPNAME"] : null;
    public static string? Key => _config.ContainsKey("KEY") ? _config["KEY"] : null;
    public static string? Region => _config.ContainsKey("REGION") ? _config["REGION"] : null;
    public static string? TtsEndpoint => _config.ContainsKey("TTS_ENDPOINT") ? _config["TTS_ENDPOINT"] : null;
    public static string? VoicesEndpoint => _config.ContainsKey("VOICES_ENDPOINT") ? _config["VOICES_ENDPOINT"] : null;
    public static string? RecognitionEndpoint => _config.ContainsKey("RECOGNITION_ENDPOINT") ? _config["RECOGNITION_ENDPOINT"] : null;

    private static Dictionary<string, string> _config = new();

    static AzureConfig()
    {
        if (!File.Exists(FILENAME))
            File.WriteAllText(FILENAME, string.Join("", CONFIGKEYS.Select(x => $"{x}\n")));
        var configStr = File.ReadAllText(FILENAME);
        foreach (var line in configStr.Replace("\r", "").Split("\n"))
        {
            int equalsIdx = line.IndexOf("=");
            if (equalsIdx < 0) continue;
            _config[line.Substring(0, equalsIdx)] = line.Substring(equalsIdx + 1);
        }
    }
}
