namespace GptAdventure.Engine;

using System.Diagnostics;
using GptAdventure.Math;

public class Game
{
    private static readonly CharInfo[] _sky_colors = new[]
    {
        new CharInfo(RenderBuffer.PIXEL_SOLID, RenderBuffer.FG_DARK_BLUE),
        new CharInfo(RenderBuffer.PIXEL_THREEQUARTERS, (ushort)(RenderBuffer.BG_BLUE | RenderBuffer.FG_DARK_BLUE)),
        new CharInfo(RenderBuffer.PIXEL_HALF, (ushort)(RenderBuffer.BG_BLUE | RenderBuffer.FG_DARK_BLUE)),
        new CharInfo(RenderBuffer.PIXEL_QUARTER, (ushort)(RenderBuffer.BG_BLUE | RenderBuffer.FG_DARK_BLUE)),

        new CharInfo(RenderBuffer.PIXEL_SOLID, RenderBuffer.FG_BLUE),
        new CharInfo(RenderBuffer.PIXEL_THREEQUARTERS, (ushort)(RenderBuffer.BG_DARK_CYAN | RenderBuffer.FG_BLUE)),
        new CharInfo(RenderBuffer.PIXEL_HALF, (ushort)(RenderBuffer.BG_DARK_CYAN | RenderBuffer.FG_BLUE)),
        new CharInfo(RenderBuffer.PIXEL_QUARTER, (ushort)(RenderBuffer.BG_DARK_CYAN | RenderBuffer.FG_BLUE)),

        new CharInfo(RenderBuffer.PIXEL_SOLID, RenderBuffer.FG_DARK_CYAN),
        new CharInfo(RenderBuffer.PIXEL_THREEQUARTERS, (ushort)(RenderBuffer.BG_CYAN | RenderBuffer.FG_DARK_CYAN)),
        new CharInfo(RenderBuffer.PIXEL_HALF, (ushort)(RenderBuffer.BG_CYAN | RenderBuffer.FG_DARK_CYAN)),
        new CharInfo(RenderBuffer.PIXEL_QUARTER, (ushort)(RenderBuffer.BG_CYAN | RenderBuffer.FG_DARK_CYAN)),

        new CharInfo(RenderBuffer.PIXEL_SOLID, RenderBuffer.FG_CYAN),
        new CharInfo(RenderBuffer.PIXEL_THREEQUARTERS, (ushort)(RenderBuffer.BG_WHITE | RenderBuffer.FG_CYAN)),
        new CharInfo(RenderBuffer.PIXEL_HALF, (ushort)(RenderBuffer.BG_WHITE | RenderBuffer.FG_CYAN)),
        new CharInfo(RenderBuffer.PIXEL_QUARTER, (ushort)(RenderBuffer.BG_WHITE | RenderBuffer.FG_CYAN)),

        new CharInfo(RenderBuffer.PIXEL_SOLID, RenderBuffer.FG_WHITE),
        new CharInfo(RenderBuffer.PIXEL_SOLID, RenderBuffer.FG_WHITE),
        new CharInfo(RenderBuffer.PIXEL_SOLID, RenderBuffer.FG_WHITE),
        new CharInfo(RenderBuffer.PIXEL_SOLID, RenderBuffer.FG_WHITE),
    };

    public Scene Scene;
    public string Title = "Kanawanagasaki.ConsoleEngine";
    public readonly Inputs Inputs;
    public readonly RenderBuffer Renderer;
    public bool IsRunning { get; private set; } = false;

    public Game(short width, short height, short pixelWidth, short pixelHeight)
    {
        Inputs = new Inputs();
        Inputs.Init();
        Renderer = new RenderBuffer(width, height, pixelWidth, pixelHeight);
        Scene = new Scene(this);
    }

    public void Run()
    {
        IsRunning = true;

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        long lastTick = 0;
        while (IsRunning)
        {
            var time = stopwatch.ElapsedMilliseconds / 1000f;
            var secondsDiff = (stopwatch.ElapsedMilliseconds - lastTick) / 1000f;
            lastTick = stopwatch.ElapsedMilliseconds;
            Native.SetConsoleTitle($"{Title} {1 / secondsDiff:0.0} FPS");

            Inputs.Tick();

            Scene.Tick(time, secondsDiff);

            Renderer.Clear();

            for (int i = 0; i < Renderer.Height; i++)
                Renderer.DrawLine(0, i, -1, Renderer.Width - 1, i, -1, _sky_colors[(int)((float)i / Renderer.Height * _sky_colors.Length)]);

            Scene.Render();

            if (Inputs.Is4Pressed)
                Renderer.DebugRenderDepth();
            else
                Renderer.Render();

            if (Inputs.Is1JustPressed)
                Renderer.DebugScreenshot();
            if (Inputs.Is2JustPressed)
                Renderer.DebugDepthScreenshot();
        }

        stopwatch.Stop();
    }
}
