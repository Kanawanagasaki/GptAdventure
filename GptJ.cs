namespace GptAdventure;

using System.Diagnostics;
using System.Text;

public class GptJ
{
    public bool IsReady { get; private set; } = false;
    public bool IsWaitingForPromt { get; private set; } = false;
    public string LastPrompt { get; private set; } = "";
    public string LastResult { get; private set; } = "";

    public event Action<string>? OnResult;

    private Process _process;
    private bool _isRunning = false;
    private CancellationTokenSource _cancellationSource;

    public GptJ()
    {
        _process = new Process();
        _process.StartInfo.FileName = "python";
        _process.StartInfo.ArgumentList.Add("gpt-j.py");
        _process.StartInfo.RedirectStandardInput = true;
        _process.StartInfo.RedirectStandardOutput = true;
        _process.StartInfo.RedirectStandardError = true;
        _process.StartInfo.UseShellExecute = false;

        _cancellationSource = new CancellationTokenSource();
    }

    public void Start()
    {
        if (_isRunning)
            return;
        _process.Start();
        ReadStdError();
        ReadStdOutput();
        _isRunning = true;
    }

    public void Stop()
    {
        if (!_isRunning)
            return;

        _cancellationSource.Cancel();
        _process.Kill();
        _isRunning = false;
    }

    public void Generate(string prompt)
    {
        if (!IsReady || !IsWaitingForPromt)
            return;

        IsWaitingForPromt = false;

        LastPrompt = prompt;
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(prompt));
        _process.StandardInput.WriteLine(base64);
    }

    private async void ReadStdError()
    {
        var buffer = new char[2048];
        int read;
        while ((read = await _process.StandardError.ReadAsync(buffer, _cancellationSource.Token)) != 0)
            if (IsReady)
                break;
            else
                Console.Write(new string(buffer, 0, read));

        string? line;
        while (!string.IsNullOrEmpty(line = await _process.StandardError.ReadLineAsync()))
            Logger.WriteLine("<GptJ> " + line);
    }

    private async void ReadStdOutput()
    {
        string? line;
        while (!string.IsNullOrEmpty(line = await _process.StandardOutput.ReadLineAsync()))
        {
            if (line == "<waiting_for_prompt>")
            {
                IsReady = true;
                IsWaitingForPromt = true;
            }
            else if (line.StartsWith("<gpt_result>"))
            {
                try
                {
                    var b64 = Convert.FromBase64String(line.Substring("<gpt_result>".Length));
                    LastResult = Encoding.UTF8.GetString(b64);

                    Logger.WriteLine("<GptJ> Generated result: " + LastResult);

                    OnResult?.Invoke(LastResult);
                }
                catch { }
            }
        }
    }
}
