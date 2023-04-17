namespace GptAdventure;

using NAudio.Wave;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.Json;

// Q: Why you use http enpoints and not nuger library?
// A: Because nuget library do not work

public class AzureTts
{
    public bool IsRunning;

    public string DefaultVoice { get; private set; }
    public string DefaultVoiceVolume { get; private set; } = "40";
    public string DefaultVoicePitch { get; private set; } = "0";
    public string DefaultVoiceRate { get; private set; } = "1";

    public event Action? OnFinishSpeacking;

    private ConcurrentQueue<string> _textToRead = new();
    private AzureTtsVoiceInfo[]? _cachedVoices = null;

    public AzureTts(string defaultVoice)
    {
        IsRunning = true;
        DefaultVoice = defaultVoice;

        if (string.IsNullOrWhiteSpace(AzureConfig.AppName))
            throw new Exception("APPNAME was null");
        if (string.IsNullOrWhiteSpace(AzureConfig.Key))
            throw new Exception("KEY was null");
        if (string.IsNullOrWhiteSpace(AzureConfig.Region))
            throw new Exception("REGION was null");
        if (string.IsNullOrWhiteSpace(AzureConfig.TtsEndpoint))
            throw new Exception("TTS_ENDPOINT was null");
        if (string.IsNullOrWhiteSpace(AzureConfig.VoicesEndpoint))
            throw new Exception("VOICES_ENDPOINT was null");

        Task.Run(async () =>
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AzureConfig.Key);
            http.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
            http.DefaultRequestHeaders.Add("User-Agent", AzureConfig.AppName);

            while (IsRunning)
            {
                if (_textToRead.Count > 0 && _textToRead.TryDequeue(out var text))
                {
                    Logger.WriteLine("<AzureTts> Saying: " + text);

                    string ssml = "";

                    int num = 0;
                    double dnum = 0;
                    var voice = DefaultVoice;
                    var volume = int.TryParse(DefaultVoiceVolume, out num) ? num : 50;
                    var pitch = int.TryParse(DefaultVoicePitch, out num) ? num : 0;
                    var rate = (double.TryParse(DefaultVoiceRate.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out dnum) ? dnum : 1).ToString("0.00").Replace(",", ".");

                    ssml = $@"
                        <speak version=""1.0"" xmlns=""http://www.w3.org/2001/10/synthesis"" xml:lang=""en-US"">
                            <voice name=""{voice}"">
                                <prosody volume=""100"" pitch=""{(pitch < 0 ? "" : "+")}{pitch}Hz"" rate=""{rate}"">
                                    {text}
                                </prosody>
                            </voice>
                        </speak>";

                    var content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");
                    var response = await http.PostAsync(AzureConfig.TtsEndpoint, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using var audioStream = await response.Content.ReadAsStreamAsync();
                        using var audioProvider = new RawSourceWaveStream(audioStream, new WaveFormat(24000, 16, 1));
                        using var waveOut = new WaveOutEvent();
                        waveOut.Init(audioProvider);
                        waveOut.Play();
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                            await Task.Delay(1000);
                        OnFinishSpeacking?.Invoke();
                    }
                    else Logger.WriteLine($"<AzureTts> Failed to fetch audio file for tts: {(int)response.StatusCode} {response.StatusCode}");
                }
                else await Task.Delay(1000);
            }
        });
    }

    public void AddTextToRead(string text)
    {
        if (string.IsNullOrWhiteSpace(AzureConfig.Key))
            Logger.WriteLine("<AzureTts> Key is empty, enter it to use TTS");
        else if (string.IsNullOrWhiteSpace(AzureConfig.Region))
            Logger.WriteLine("<AzureTts> Region is empty, enter it to use TTS");
        else
            lock (_textToRead)
                _textToRead.Enqueue(text);
    }

    public async Task<AzureTtsVoiceInfo[]> GetVoices()
    {
        if (_cachedVoices is not null)
            return _cachedVoices;

        if (string.IsNullOrWhiteSpace(AzureConfig.Key))
            return Array.Empty<AzureTtsVoiceInfo>();

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AzureConfig.Key);
        var response = await http.GetAsync(AzureConfig.VoicesEndpoint);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var json = await response.Content.ReadAsStringAsync();
            _cachedVoices = JsonSerializer.Deserialize<AzureTtsVoiceInfo[]>(json);
            return _cachedVoices ?? Array.Empty<AzureTtsVoiceInfo>();
        }
        else return Array.Empty<AzureTtsVoiceInfo>();
    }

    public class AzureTtsVoiceInfo
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? LocalName { get; set; }
        public string? ShortName { get; set; }
        public string? Gender { get; set; }
        public string? Locale { get; set; }
        public string? LocaleName { get; set; }
        public string? SampleRateHertz { get; set; }
        public string? VoiceType { get; set; }
        public string? Status { get; set; }
        public string? WordsPerMinute { get; set; }
    }

}
