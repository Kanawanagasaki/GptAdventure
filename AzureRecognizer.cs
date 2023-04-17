namespace GptAdventure;

using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Text.Json;

// Q: Why you use http enpoints and not nuger library?
// A: Because nuget library is causing my windows to crash. It is real, microsoft somehow failed to make library for their service.

public class AzureRecognizer
{
    private static readonly TimeSpan SILENCE_TIME = TimeSpan.FromSeconds(1);
    private const float VOLUME_THRESHOLD = 0.1f;

    public IReadOnlyList<string> Results => _results;
    public event Action<string>? OnNewResult;

    private MemoryStream _waveStream;
    private WaveInEvent _waveSource;

    private HttpClient _http;

    private List<string> _results = new();

    private DateTime _lastSampleCapture = DateTime.UnixEpoch;
    private bool _captured = false;

    private bool _isActive = false;

    public AzureRecognizer()
    {
        if (string.IsNullOrWhiteSpace(AzureConfig.AppName))
            throw new Exception("APPNAME was null");
        if (string.IsNullOrWhiteSpace(AzureConfig.Key))
            throw new Exception("KEY was null");
        if (string.IsNullOrWhiteSpace(AzureConfig.Region))
            throw new Exception("REGION was null");
        if (string.IsNullOrWhiteSpace(AzureConfig.RecognitionEndpoint))
            throw new Exception("RECOGNITION_ENDPOINT was null");

        _waveStream = new MemoryStream();

        _waveSource = new WaveInEvent();
        _waveSource.WaveFormat = new(16000, 16, 1);
        _waveSource.DataAvailable += OnRecordData;
        _waveSource.StartRecording();

        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AzureConfig.Key);
        _http.DefaultRequestHeaders.Add("User-Agent", AzureConfig.AppName);
    }

    public void Activate()
    {
        _isActive = true;
        _waveStream.SetLength(0);
        _captured = false;
    }

    public void Deactivate()
    {
        _isActive = false;
        _waveStream.SetLength(0);
        _captured = false;
    }

    private void OnRecordData(object? sender, WaveInEventArgs args)
    {
        if (!_isActive)
            return;

        var volume = 0f;
        for (int index = 0; index < args.BytesRecorded; index += 2)
        {
            var sample = (short)((args.Buffer[index + 1] << 8) | args.Buffer[index + 0]);
            var sample32 = MathF.Abs(sample / 32768f);
            if (volume < sample32)
                volume = sample32;
        }

        var now = DateTime.Now;
        var captureDiff = now - _lastSampleCapture;

        if (VOLUME_THRESHOLD < volume || captureDiff < SILENCE_TIME)
        {
            _waveStream.Write(args.Buffer, 0, args.BytesRecorded);
            _captured = true;
            if (VOLUME_THRESHOLD < volume)
                _lastSampleCapture = now;
        }
        else if (_captured)
        {
            var buffer = _waveStream.ToArray();
            Recognize(buffer);

            _waveStream.SetLength(0);
            _captured = false;
        }
    }

    private async void Recognize(byte[] buffer)
    {
        var post = new ByteArrayContent(buffer);
        post.Headers.TryAddWithoutValidation("Content-Type", "audio/wav; codecs=audio/pcm; samplerate=16000");
        var response = await _http.PostAsync(AzureConfig.RecognitionEndpoint + "?language=en-US", post);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RecognitionResult>(content);
            if (result?.RecognitionStatus == "Success")
            {
                Logger.WriteLine("<AzureRecognizer> Recognized text: " + result.DisplayText);

                _results.Add(result.DisplayText);
                OnNewResult?.Invoke(result.DisplayText);
            }
        }
    }

    public record RecognitionResult(string RecognitionStatus, long Offset, long Duration, string DisplayText);
}
