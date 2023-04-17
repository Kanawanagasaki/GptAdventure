namespace GptAdventure.Engine;

using System.Drawing;
using System.Runtime.InteropServices;

public class Texture
{
    private static readonly Dictionary<Color, CharInfo> _colorMap = new();

    public readonly CharInfo[,] Pixels;
    public readonly int Width;
    public readonly int Height;

    public Texture(CharInfo[,] pixels)
    {
        Pixels = pixels;
        Width = pixels.GetLength(1);
        Height = pixels.GetLength(0);
    }

    public CharInfo Sample(float u, float v)
    {
        v = 1f - v;
        if (u < 0) u -= (int)(u - 1f);
        if (v < 0) v -= (int)(v - 1f);

        var x = (int)(u * Width) % Width;
        var y = (int)(v * Height) % Height;
        return Pixels[y, x];
    }

    static Texture()
    {
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    (int r, int g, int b) fg = i switch
                    {
                        0x0 => (0x00, 0x00, 0x00), // BLACK
                        0x1 => (0x00, 0x00, 0x8B), // DARK_BLUE
                        0x2 => (0x01, 0x32, 0x20), // DARK_GREEN
                        0x3 => (0x00, 0x8B, 0x8B), // DARK_CYAN
                        0x4 => (0x8B, 0x00, 0x00), // DARK_RED
                        0x5 => (0x8B, 0x00, 0x8B), // DARK_MAGENTA
                        0x6 => (0x79, 0x61, 0x17), // DARK_YELLOW
                        0x7 => (0xA9, 0xA9, 0xA9), // GREY
                        0x8 => (0x80, 0x80, 0x80), // DARK_GREY
                        0x9 => (0x00, 0x00, 0xAA), // BLUE
                        0xA => (0x00, 0xAA, 0x00), // GREEN
                        0xB => (0x00, 0xAA, 0xAA), // CYAN
                        0xC => (0xAA, 0x00, 0x00), // RED
                        0xD => (0xAA, 0x00, 0xAA), // MAGENTA
                        0xE => (0xAA, 0xAA, 0x00), // YELLOW
                        0xF => (0xFF, 0xFF, 0xFF), // WHITE
                        _ => (0x00, 0x00, 0x00)
                    };
                    (int r, int g, int b) bg = j switch
                    {
                        0x0 => (0x00, 0x00, 0x00), // BLACK
                        0x1 => (0x00, 0x00, 0x8B), // DARK_BLUE
                        0x2 => (0x01, 0x32, 0x20), // DARK_GREEN
                        0x3 => (0x00, 0x8B, 0x8B), // DARK_CYAN
                        0x4 => (0x8B, 0x00, 0x00), // DARK_RED
                        0x5 => (0x8B, 0x00, 0x8B), // DARK_MAGENTA
                        0x6 => (0x79, 0x61, 0x17), // DARK_YELLOW
                        0x7 => (0xA9, 0xA9, 0xA9), // GREY
                        0x8 => (0x80, 0x80, 0x80), // DARK_GREY
                        0x9 => (0x00, 0x00, 0xAA), // BLUE
                        0xA => (0x00, 0xAA, 0x00), // GREEN
                        0xB => (0x00, 0xAA, 0xAA), // CYAN
                        0xC => (0xAA, 0x00, 0x00), // RED
                        0xD => (0xAA, 0x00, 0xAA), // MAGENTA
                        0xE => (0xAA, 0xAA, 0x00), // YELLOW
                        0xF => (0xFF, 0xFF, 0xFF), // WHITE
                        _ => (0x00, 0x00, 0x00)
                    };

                    var ratio = k switch
                    {
                        0 => 0f,
                        1 => 0.25f,
                        2 => 0.5f,
                        3 => 0.5f,
                        _ => 0f
                    };

                    var r = (int)(fg.r * (1f - ratio) + bg.r * ratio);
                    var g = (int)(fg.g * (1f - ratio) + bg.g * ratio);
                    var b = (int)(fg.b * (1f - ratio) + bg.b * ratio);

                    var ch = k switch
                    {
                        0 => RenderBuffer.PIXEL_SOLID,
                        1 => RenderBuffer.PIXEL_THREEQUARTERS,
                        2 => RenderBuffer.PIXEL_HALF,
                        3 => RenderBuffer.PIXEL_QUARTER,
                        _ => RenderBuffer.PIXEL_SOLID
                    };

                    _colorMap[Color.FromArgb(r, g, b)] = new CharInfo(ch, (ushort)(i | (j << 4)));
                }
            }
        }
    }

    private static CharInfo FindClosestColor(Color target)
    {
        var (r, g, b) = (target.R, target.G, target.B);

        var bestDist = float.MaxValue;
        var bestColor = new CharInfo(RenderBuffer.PIXEL_SOLID, RenderBuffer.FG_WHITE);

        foreach (var kv in _colorMap)
        {
            var c = kv.Key;
            var dist = (c.R - r) * (c.R - r) + (c.G - g) * (c.G - g) + (c.B - b) * (c.B - b);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestColor = kv.Value;
            }
        }

        return bestColor;
    }

    public static Texture FromFile(string path)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("Windows only");

        using var btm = new Bitmap(path);
        var pixels = new CharInfo[btm.Height, btm.Width];
        for (int iy = 0; iy < btm.Height; iy++)
        {
            for (int ix = 0; ix < btm.Width; ix++)
            {
                var color = btm.GetPixel(ix, iy);
                pixels[iy, ix] = FindClosestColor(color);
            }
        }
        return new Texture(pixels);
    }
}
