namespace GptAdventure.Engine;

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public class RenderBuffer
{
    #region fields

    public const int PIXEL_WIDTH = 15;
    public const int PIXEL_HEIGHT = 20;
    public const float PIXEL_RATIO = (float)PIXEL_WIDTH / (float)PIXEL_HEIGHT;

    public const char PIXEL_SOLID = (char)0xDB; // █
    public const char PIXEL_THREEQUARTERS = (char)0xB2; // ▓
    public const char PIXEL_HALF = (char)0xB1; // ▒
    public const char PIXEL_QUARTER = (char)0xB0; // ░

    public const ushort FG_BLACK = 0x0000;
    public const ushort FG_DARK_BLUE = 0x0001;
    public const ushort FG_DARK_GREEN = 0x0002;
    public const ushort FG_DARK_CYAN = 0x0003;
    public const ushort FG_DARK_RED = 0x0004;
    public const ushort FG_DARK_MAGENTA = 0x0005;
    public const ushort FG_DARK_YELLOW = 0x0006;
    public const ushort FG_GREY = 0x0007;
    public const ushort FG_DARK_GREY = 0x0008;
    public const ushort FG_BLUE = 0x0009;
    public const ushort FG_GREEN = 0x000A;
    public const ushort FG_CYAN = 0x000B;
    public const ushort FG_RED = 0x000C;
    public const ushort FG_MAGENTA = 0x000D;
    public const ushort FG_YELLOW = 0x000E;
    public const ushort FG_WHITE = 0x000F;
    public const ushort BG_BLACK = 0x0000;
    public const ushort BG_DARK_BLUE = 0x0010;
    public const ushort BG_DARK_GREEN = 0x0020;
    public const ushort BG_DARK_CYAN = 0x0030;
    public const ushort BG_DARK_RED = 0x0040;
    public const ushort BG_DARK_MAGENTA = 0x0050;
    public const ushort BG_DARK_YELLOW = 0x0060;
    public const ushort BG_GREY = 0x0070;
    public const ushort BG_DARK_GREY = 0x0080;
    public const ushort BG_BLUE = 0x0090;
    public const ushort BG_GREEN = 0x00A0;
    public const ushort BG_CYAN = 0x00B0;
    public const ushort BG_RED = 0x00C0;
    public const ushort BG_MAGENTA = 0x00D0;
    public const ushort BG_YELLOW = 0x00E0;
    public const ushort BG_WHITE = 0x00F0;

    private static Dictionary<char, string[]> ALPHABET = new()
    {
        {
            'A',
            """
             █████ 
            ██   ██
            ███████
            ██   ██
            ██   ██
            """.Split("\n")
        },
        {
            'B',
            """
            ██████ 
            ██   ██
            ██████ 
            ██   ██
            ██████ 
            """.Split("\n")
        },
        {
            'C',
            """
             █████
            ██    
            ██    
            ██    
             █████
            """.Split("\n")
        },
        {
            'D',
            """
            ██████ 
            ██   ██
            ██   ██
            ██   ██
            ██████ 
            """.Split("\n")
        },
        {
            'E',
            """
            ███████
            ██     
            █████  
            ██     
            ███████
            """.Split("\n")
        },
        {
            'F',
            """
            ███████
            ██     
            █████  
            ██     
            ██     
            """.Split("\n")
        },
        {
            'G',
            """
             ██████ 
            ██      
            ██   ███
            ██    ██
             ██████ 
            """.Split("\n")
        },
        {
            'H',
            """
            ██   ██
            ██   ██
            ███████
            ██   ██
            ██   ██
            """.Split("\n")
        },
        {
            'I',
            """
            ██
            ██
            ██
            ██
            ██
            """.Split("\n")
        },
        {
            'J',
            """
                 ██
                 ██
                 ██
            ██   ██
             █████ 
            """.Split("\n")
        },
        {
            'K',
            """
            ██   ██
            ██  ██ 
            █████  
            ██  ██ 
            ██   ██
            """.Split("\n")
        },
        {
            'L',
            """
            ██     
            ██     
            ██     
            ██     
            ███████
            """.Split("\n")
        },
        {
            'M',
            """
            ███    ███
            ████  ████
            ██ ████ ██
            ██  ██  ██
            ██      ██
            """.Split("\n")
        },
        {
            'N',
            """
            ███    ██
            ████   ██
            ██ ██  ██
            ██  ██ ██
            ██   ████
            """.Split("\n")
        },
        {
            'O',
            """
             ██████ 
            ██    ██
            ██    ██
            ██    ██
             ██████ 
            """.Split("\n")
        },
        {
            'P',
            """
            ██████ 
            ██   ██
            ██████ 
            ██     
            ██     
            """.Split("\n")
        },
        {
            'Q',
            """
             ██████ 
            ██    ██
            ██    ██
            ██ ▄▄ ██
             ██████ 
            """.Split("\n")
        },
        {
            'R',
            """
            ██████ 
            ██   ██
            ██████ 
            ██   ██
            ██   ██
            """.Split("\n")
        },
        {
            'S',
            """
            ███████
            ██     
            ███████
                 ██
            ███████
            """.Split("\n")
        },
        {
            'T',
            """
            ████████
               ██   
               ██   
               ██   
               ██   
            """.Split("\n")
        },
        {
            'U',
            """
            ██    ██
            ██    ██
            ██    ██
            ██    ██
             ██████ 
            """.Split("\n")
        },
        {
            'V',
            """
            ██    ██
            ██    ██
            ██    ██
             ██  ██ 
              ████  
            """.Split("\n")
        },
        {
            'W',
            """
            ██     ██
            ██     ██
            ██  █  ██
            ██ ███ ██
             ███ ███ 
            """.Split("\n")
        },
        {
            'X',
            """
            ██   ██
             ██ ██ 
              ███  
             ██ ██ 
            ██   ██
            """.Split("\n")
        },
        {
            'Y',
            """
            ██    ██
             ██  ██ 
              ████  
               ██   
               ██   
            """.Split("\n")
        },
        {
            'Z',
            """
            ███████
               ███ 
              ███  
             ███   
            ███████
            """.Split("\n")
        },
        {
            '0',
            """
             ██████ 
            ██  ████
            ██ ██ ██
            ████  ██
             ██████ 
            """.Split("\n")
        },
        {
            '1',
            """
             ██
            ███
             ██
             ██
             ██
            """.Split("\n")
        },
        {
            '2',
            """
            ██████ 
                 ██
             █████ 
            ██     
            ███████
            """.Split("\n")
        },
        {
            '3',
            """
            ██████
                 █
             █████
                 █
            ██████
            """.Split("\n")
        },
        {
            '4',
            """
            ██   ██
            ██   ██
            ███████
                 ██
                 ██
            """.Split("\n")
        },
        {
            '5',
            """
            ███████
            ██     
            ███████
                 ██
            ███████
            """.Split("\n")
        },
        {
            '6',
            """
             ██████ 
            ██      
            ███████ 
            ██    ██
             ██████ 
            """.Split("\n")
        },
        {
            '7',
            """
            ███████
                 ██
                ██ 
               ██  
               ██  
            """.Split("\n")
        },
        {
            '8',
            """
             █████ 
            ██   ██
             █████ 
            ██   ██
             █████ 
            """.Split("\n")
        },
        {
            '9',
            """
             █████ 
            ██   ██
             ██████
                 ██
             █████ 
            """.Split("\n")
        },
        {
            '-',
            """
                 
                 
            █████
                 
                 
            """.Split("\n")
        },
        {
            '_',
            """
                   
                   
                   
                   
            ███████
            """.Split("\n")
        },
        {
            ':',
            """
              
            ██
              
            ██
              
            """.Split("\n")
        },
        {
            ' ',
            """
                 
                 
                 
                 
                 
            """.Split("\n")
        },
        {
            '.',
            """
              
              
              
              
            ██
            """.Split("\n")
        },
        {
            ',',
            """
              
              
              
              
            ██ 
            """.Split("\n")
        }
    };

    public int Width;
    public int Height;
    public SmallRect CanvasSize;
    private CharInfo[] _buffer = Array.Empty<CharInfo>();
    private float[] _depthBuffer = Array.Empty<float>();
    private nint _handle;
    #endregion

    public CharInfo this[int offset]
    {
        get => _buffer[offset];
        set => _buffer[offset] = value;
    }

    public CharInfo this[int row, int column]
    {
        get => _buffer[row * Width + column];
        set => _buffer[row * Width + column] = value;
    }

    public RenderBuffer(short screenWidth, short screenHeight, short fontWidth, short fontHeight)
    {
        _handle = Native.GetStdHandle(Native.STD_OUTPUT_HANDLE);
        Native.SetConsoleOutputCP(850);
        Native.SetConsoleCP(850);

        var rectWindow = new SmallRect
        {
            Left = 0,
            Top = 0,
            Right = 1,
            Bottom = 1
        };
        Native.SetConsoleWindowInfo(_handle, true, ref rectWindow);

        var coord = new ConsoleCoord { X = screenWidth, Y = screenHeight };
        Native.SetConsoleScreenBufferSize(_handle, coord);
        Native.SetConsoleActiveScreenBuffer(_handle);

        var fontInfo = new ConsoleFontInfo
        {
            nFont = 0,
            dwFontSize = new ConsoleCoord { X = fontWidth, Y = fontHeight },
            FontFamily = 0x00,
            FontWeight = 400
        };
        fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
        Native.SetCurrentConsoleFontEx(_handle, false, ref fontInfo);

        rectWindow = new SmallRect
        {
            Left = 0,
            Top = 0,
            Right = (short)(screenWidth - 1),
            Bottom = (short)(screenHeight - 1)
        };
        Native.SetConsoleWindowInfo(_handle, true, ref rectWindow);

        Native.SetConsoleMode(_handle, Native.ConsoleModes.ENABLE_EXTENDED_FLAGS | Native.ConsoleModes.ENABLE_WINDOW_INPUT | Native.ConsoleModes.ENABLE_MOUSE_INPUT);

        Width = screenWidth;
        Height = screenHeight;
        _buffer = new CharInfo[Width * Height];
        _depthBuffer = new float[Width * Height];
        Clear();
    }

    public void Clear()
    {
        for (int i = 0; i < _buffer.Length; i++)
        {
            _buffer[i] = new CharInfo();
            _depthBuffer[i] = float.MinValue;
        }
    }

    public void DrawText(string text, ushort attr, int x, int y)
    {
        text = text.ToUpper();

        var lines = text.Split("\n");
        int iy = y;
        foreach (var line in lines)
        {
            int ix = x;
            int ayMax = 0;
            foreach (var ch in line)
            {
                if (!ALPHABET.ContainsKey(ch))
                    continue;

                var ascii = ALPHABET[ch];
                int axMax = 0;
                for (int ay = 0; ay < ascii.Length; ay++)
                {
                    for (int ax = 0; ax < ascii[ay].Length; ax++)
                    {
                        if (ascii[ay][ax] == '█')
                            DrawPoint(ix + ax, iy + ay, float.MaxValue, new CharInfo(PIXEL_SOLID, attr));
                    }
                    if (axMax < ascii[ay].Length) axMax = ascii[ay].Length;
                }
                if (ayMax < ascii.Length) ayMax = ascii.Length;
                ix += axMax + 1;
            }
            iy += ayMax + 1;
        }
    }

    public void FillTriangle(QueuedTriangle triangle, bool texture)
    {
        if (texture && triangle.Obj.Texture is null)
            throw new NullReferenceException(nameof(triangle.Obj.Texture));

        var (vec1, vec2, vec3) = (triangle.Triangle.P1, triangle.Triangle.P2, triangle.Triangle.P3);
        var (uv1, uv2, uv3) = (triangle.Triangle.TC1, triangle.Triangle.TC2, triangle.Triangle.TC3);

        if (vec2.Y < vec1.Y) { (vec2, vec1) = (vec1, vec2); (uv2, uv1) = (uv1, uv2); }
        if (vec3.Y < vec1.Y) { (vec3, vec1) = (vec1, vec3); (uv3, uv1) = (uv1, uv3); }
        if (vec3.Y < vec2.Y) { (vec3, vec2) = (vec2, vec3); (uv3, uv2) = (uv2, uv3); }

        var (x1, y1, w1) = ((int)vec1.X, (int)vec1.Y, vec1.W);
        var (x2, y2, w2) = ((int)vec2.X, (int)vec2.Y, vec2.W);
        var (x3, y3, w3) = ((int)vec3.X, (int)vec3.Y, vec3.W);

        var (u1, v1) = (uv1.U, uv1.V);
        var (u2, v2) = (uv2.U, uv2.V);
        var (u3, v3) = (uv3.U, uv3.V);

        int dy1 = y2 - y1;
        int dx1 = x2 - x1;
        float dv1 = v2 - v1;
        float du1 = u2 - u1;
        float dw1 = w2 - w1;

        int dy2 = y3 - y1;
        int dx2 = x3 - x1;
        float dv2 = v3 - v1;
        float du2 = u3 - u1;
        float dw2 = w3 - w1;

        float tex_u, tex_v, tex_w;

        float dax_step = 0, dbx_step = 0,
            du1_step = 0, dv1_step = 0,
            du2_step = 0, dv2_step = 0,
            dw1_step = 0, dw2_step = 0;

        if (dy1 != 0) dax_step = dx1 / (float)MathF.Abs(dy1);
        if (dy2 != 0) dbx_step = dx2 / (float)MathF.Abs(dy2);

        if (dy1 != 0) du1_step = du1 / MathF.Abs(dy1);
        if (dy1 != 0) dv1_step = dv1 / MathF.Abs(dy1);
        if (dy1 != 0) dw1_step = dw1 / MathF.Abs(dy1);

        if (dy2 != 0) du2_step = du2 / MathF.Abs(dy2);
        if (dy2 != 0) dv2_step = dv2 / MathF.Abs(dy2);
        if (dy2 != 0) dw2_step = dw2 / MathF.Abs(dy2);

        if (dy1 != 0)
        {
            for (int i = y1; i <= y2; i++)
            {
                int ax = (int)(x1 + (i - y1) * dax_step);
                int bx = (int)(x1 + (i - y1) * dbx_step);

                float tex_su = u1 + (i - y1) * du1_step;
                float tex_sv = v1 + (i - y1) * dv1_step;
                float tex_sw = w1 + (i - y1) * dw1_step;

                float tex_eu = u1 + (i - y1) * du2_step;
                float tex_ev = v1 + (i - y1) * dv2_step;
                float tex_ew = w1 + (i - y1) * dw2_step;

                if (ax > bx)
                {
                    (ax, bx) = (bx, ax);
                    (tex_su, tex_eu) = (tex_eu, tex_su);
                    (tex_sv, tex_ev) = (tex_ev, tex_sv);
                    (tex_sw, tex_ew) = (tex_ew, tex_sw);
                }

                tex_u = tex_su;
                tex_v = tex_sv;
                tex_w = tex_sw;

                float tstep = 1.0f / ((float)(bx - ax));
                float t = 0.0f;

                for (int j = ax; j < bx; j++)
                {
                    tex_u = (1.0f - t) * tex_su + t * tex_eu;
                    tex_v = (1.0f - t) * tex_sv + t * tex_ev;
                    tex_w = (1.0f - t) * tex_sw + t * tex_ew;

                    var charInfo = texture ? triangle.Obj.Texture!.Sample(tex_u / tex_w, tex_v / tex_w) : triangle.ChInfo;
                    DrawPoint(j, i, tex_w, charInfo);

                    t += tstep;
                }

            }
        }

        dy1 = y3 - y2;
        dx1 = x3 - x2;
        dv1 = v3 - v2;
        du1 = u3 - u2;
        dw1 = w3 - w2;

        if (dy1 != 0) dax_step = dx1 / (float)MathF.Abs(dy1);
        if (dy2 != 0) dbx_step = dx2 / (float)MathF.Abs(dy2);

        du1_step = 0;
        dv1_step = 0;
        if (dy1 != 0) du1_step = du1 / MathF.Abs(dy1);
        if (dy1 != 0) dv1_step = dv1 / MathF.Abs(dy1);
        if (dy1 != 0) dw1_step = dw1 / MathF.Abs(dy1);

        if (dy1 != 0)
        {
            for (int i = y2; i <= y3; i++)
            {
                int ax = (int)(x2 + (i - y2) * dax_step);
                int bx = (int)(x1 + (i - y1) * dbx_step);

                float tex_su = u2 + (i - y2) * du1_step;
                float tex_sv = v2 + (i - y2) * dv1_step;
                float tex_sw = w2 + (i - y2) * dw1_step;

                float tex_eu = u1 + (i - y1) * du2_step;
                float tex_ev = v1 + (i - y1) * dv2_step;
                float tex_ew = w1 + (i - y1) * dw2_step;

                if (ax > bx)
                {
                    (ax, bx) = (bx, ax);
                    (tex_su, tex_eu) = (tex_eu, tex_su);
                    (tex_sv, tex_ev) = (tex_ev, tex_sv);
                    (tex_sw, tex_ew) = (tex_ew, tex_sw);
                }

                tex_u = tex_su;
                tex_v = tex_sv;
                tex_w = tex_sw;

                float tstep = 1.0f / ((float)(bx - ax));
                float t = 0.0f;

                for (int j = ax; j < bx; j++)
                {
                    tex_u = (1.0f - t) * tex_su + t * tex_eu;
                    tex_v = (1.0f - t) * tex_sv + t * tex_ev;
                    tex_w = (1.0f - t) * tex_sw + t * tex_ew;

                    var charInfo = texture ? triangle.Obj.Texture!.Sample(tex_u / tex_w, tex_v / tex_w) : triangle.ChInfo;
                    DrawPoint(j, i, tex_w, charInfo);

                    t += tstep;
                }
            }
        }
    }

    public void DrawTriangle(QueuedTriangle triangle)
    {
        var (x1, y1, w1) = ((int)triangle.Triangle.P1.X, (int)triangle.Triangle.P1.Y, triangle.Triangle.P1.W);
        var (x2, y2, w2) = ((int)triangle.Triangle.P2.X, (int)triangle.Triangle.P2.Y, triangle.Triangle.P2.W);
        var (x3, y3, w3) = ((int)triangle.Triangle.P3.X, (int)triangle.Triangle.P3.Y, triangle.Triangle.P3.W);

        DrawLine(x1, y1, w1, x2, y2, w2, triangle.ChInfo);
        DrawLine(x2, y2, w2, x3, y3, w3, triangle.ChInfo);
        DrawLine(x3, y3, w3, x1, y1, w1, triangle.ChInfo);
    }

    public void DrawLine(int x1, int y1, float w1, int x2, int y2, float w2, CharInfo ci)
    {
        int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;

        dx = x2 - x1;
        dy = y2 - y1;
        dx1 = System.Math.Abs(dx);
        dy1 = System.Math.Abs(dy);
        px = 2 * dy1 - dx1;
        py = 2 * dx1 - dy1;
        if (dy1 <= dx1)
        {
            if (dx >= 0)
            {
                x = x1;
                y = y1;
                xe = x2;

                DrawPoint(x, y, w1, ci);
            }
            else
            {
                x = x2;
                y = y2;
                xe = x1;

                DrawPoint(x, y, w2, ci);
            }

            for (i = 0; x < xe; i++)
            {
                x = x + 1;
                if (px < 0)
                    px = px + 2 * dy1;
                else
                {
                    if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        y = y + 1;
                    else y = y - 1;
                    px = px + 2 * (dy1 - dx1);
                }
                DrawPoint(x, y, w1 + (1f - (xe - x) / (float)dx1) * (w2 - w1), ci);
            }
        }
        else
        {
            if (dy >= 0)
            {
                x = x1;
                y = y1;
                ye = y2;
                DrawPoint(x, y, w1, ci);
            }
            else
            {
                x = x2;
                y = y2;
                ye = y1;
                DrawPoint(x, y, w2, ci);
            }

            for (i = 0; y < ye; i++)
            {
                y = y + 1;
                if (py <= 0)
                    py = py + 2 * dx1;
                else
                {
                    if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        x = x + 1;
                    else x = x - 1;
                    py = py + 2 * (dx1 - dy1);
                }
                DrawPoint(x, y, w1 + (1f - (ye - y) / (float)dy1) * (w2 - w1), ci);
            }
        }
    }

    public void DrawPoint(int x, int y, float w, CharInfo ci)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height && _depthBuffer[y * Width + x] < w)
        {
            this[y, x] = ci;
            _depthBuffer[y * Width + x] = w;
        }
    }

    public void Render()
    {
        var rect = new SmallRect() { Left = 0, Top = 0, Right = (short)Width, Bottom = (short)Height };
        Native.WriteConsoleOutput(_handle, _buffer, new ConsoleCoord() { X = (short)Width, Y = (short)Height }, new ConsoleCoord() { X = 0, Y = 0 }, ref rect);
    }

    public void DebugRenderDepth()
    {
        var minW = _depthBuffer.Min();
        var maxW = _depthBuffer.Where(x => x != float.MaxValue).Max();
        var diffW = maxW - minW;

        var rect = new SmallRect() { Left = 0, Top = 0, Right = (short)Width, Bottom = (short)Height };
        var depthBuffer = new CharInfo[_depthBuffer.Length];
        for (int i = 0; i < _depthBuffer.Length; i++)
        {
            var w = _depthBuffer[i];
            if (w == float.MaxValue)
                depthBuffer[i] = new CharInfo(PIXEL_SOLID, (ushort)(FG_DARK_RED & 0xFFFF));
            else
            {
                var ratio = w == float.MaxValue ? 1f : (w - minW) / diffW;
                depthBuffer[i] = GetCharInfoFromLum(ratio);
            }
        }

        Native.WriteConsoleOutput(_handle, depthBuffer, new ConsoleCoord() { X = (short)Width, Y = (short)Height }, new ConsoleCoord() { X = 0, Y = 0 }, ref rect);
    }

    public void DebugScreenshot()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;
        if (!Directory.Exists("screenshots"))
            Directory.CreateDirectory("screenshots");

        using var btm = new Bitmap(Width, Height);
        for (int iy = 0; iy < Height; iy++)
        {
            for (int ix = 0; ix < Width; ix++)
            {
                var ch = this[iy, ix];
                var color = GetColorFromCharInfo(this[iy, ix]);
                btm.SetPixel(ix, iy, Color.FromArgb(color.r, color.g, color.b));
            }
        }
        btm.Save($"screenshots/{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}-{Guid.NewGuid().ToString("N")}.png", ImageFormat.Png);
    }

    public void DebugDepthScreenshot()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;
        if (!Directory.Exists("screenshots"))
            Directory.CreateDirectory("screenshots");

        var minW = _depthBuffer.Min();
        var maxW = _depthBuffer.Where(x => x != float.MaxValue).Max();
        var diffW = maxW - minW;

        using var btm = new Bitmap(Width, Height);
        for (int iy = 0; iy < Height; iy++)
        {
            for (int ix = 0; ix < Width; ix++)
            {
                var w = _depthBuffer[iy * Width + ix];
                if (w == float.MaxValue)
                    btm.SetPixel(ix, iy, Color.FromArgb(60, 0, 0));
                else
                {
                    var ratio = w == float.MaxValue ? 1f : (w - minW) / diffW;
                    btm.SetPixel(ix, iy, Color.FromArgb((int)(255f * ratio), (int)(255f * ratio), (int)(255f * ratio)));
                }
            }
        }
        btm.Save($"screenshots/depth-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}-{Guid.NewGuid().ToString("N")}.png", ImageFormat.Png);
    }

    public static CharInfo GetCharInfoFromLum(float lum)
    {
        ushort bgColor, fgColor;
        char ch;
        int pixelBw = (int)(13.0f * lum);
        switch (pixelBw)
        {
            case 0: bgColor = BG_BLACK; fgColor = FG_BLACK; ch = PIXEL_SOLID; break;

            case 1: bgColor = BG_BLACK; fgColor = FG_DARK_GREY; ch = PIXEL_QUARTER; break;
            case 2: bgColor = BG_BLACK; fgColor = FG_DARK_GREY; ch = PIXEL_HALF; break;
            case 3: bgColor = BG_BLACK; fgColor = FG_DARK_GREY; ch = PIXEL_THREEQUARTERS; break;
            case 4: bgColor = BG_BLACK; fgColor = FG_DARK_GREY; ch = PIXEL_SOLID; break;

            case 5: bgColor = BG_DARK_GREY; fgColor = FG_GREY; ch = PIXEL_QUARTER; break;
            case 6: bgColor = BG_DARK_GREY; fgColor = FG_GREY; ch = PIXEL_HALF; break;
            case 7: bgColor = BG_DARK_GREY; fgColor = FG_GREY; ch = PIXEL_THREEQUARTERS; break;
            case 8: bgColor = BG_DARK_GREY; fgColor = FG_GREY; ch = PIXEL_SOLID; break;

            case 9: bgColor = BG_GREY; fgColor = FG_WHITE; ch = PIXEL_QUARTER; break;
            case 10: bgColor = BG_GREY; fgColor = FG_WHITE; ch = PIXEL_HALF; break;
            case 11: bgColor = BG_GREY; fgColor = FG_WHITE; ch = PIXEL_THREEQUARTERS; break;
            case 12: bgColor = BG_GREY; fgColor = FG_WHITE; ch = PIXEL_SOLID; break;
            case 13: bgColor = BG_GREY; fgColor = FG_WHITE; ch = PIXEL_SOLID; break;
            default:
                bgColor = BG_BLACK; fgColor = FG_BLACK; ch = ' '; break;
        }

        return new CharInfo(ch, (ushort)(bgColor | fgColor & 0xFFFF));
    }

    private (int r, int g, int b) GetColorFromCharInfo(CharInfo info)
    {
        (int r, int g, int b) fg = (info.Attributes & 0xF) switch
        {
            0x0 => (0x00, 0x00, 0x00), // BLACK
            0x1 => (0x00, 0x00, 0x8B), // DARK_BLUE
            0x2 => (0x01, 0x32, 0x20), // DARK_GREEN
            0x3 => (0x00, 0x8B, 0x8B), // DARK_CYAN
            0x4 => (0x8B, 0x00, 0x00), // DARK_RED
            0x5 => (0x8B, 0x00, 0x8B), // DARK_MAGENTA
            0x6 => (0x79, 0x61, 0x17), // DARK_YELLOW
            0x7 => (0x80, 0x80, 0x80), // GREY
            0x8 => (0xA9, 0xA9, 0xA9), // DARK_GREY
            0x9 => (0x00, 0x00, 0xAA), // BLUE
            0xA => (0x00, 0xAA, 0x00), // GREEN
            0xB => (0x00, 0xAA, 0xAA), // CYAN
            0xC => (0xAA, 0x00, 0x00), // RED
            0xD => (0xAA, 0x00, 0xAA), // MAGENTA
            0xE => (0xAA, 0xAA, 0x00), // YELLOW
            0xF => (0xFF, 0xFF, 0xFF), // WHITE
            _ => (0x00, 0x00, 0x00)
        };
        (int r, int g, int b) bg = ((info.Attributes >> 4) & 0xF) switch
        {
            0x0 => (0x00, 0x00, 0x00), // BLACK
            0x1 => (0x00, 0x00, 0x8B), // DARK_BLUE
            0x2 => (0x01, 0x32, 0x20), // DARK_GREEN
            0x3 => (0x00, 0x8B, 0x8B), // DARK_CYAN
            0x4 => (0x8B, 0x00, 0x00), // DARK_RED
            0x5 => (0x8B, 0x00, 0x8B), // DARK_MAGENTA
            0x6 => (0x79, 0x61, 0x17), // DARK_YELLOW
            0x7 => (0x80, 0x80, 0x80), // GREY
            0x8 => (0xA9, 0xA9, 0xA9), // DARK_GREY
            0x9 => (0x00, 0x00, 0xAA), // BLUE
            0xA => (0x00, 0xAA, 0x00), // GREEN
            0xB => (0x00, 0xAA, 0xAA), // CYAN
            0xC => (0xAA, 0x00, 0x00), // RED
            0xD => (0xAA, 0x00, 0xAA), // MAGENTA
            0xE => (0xAA, 0xAA, 0x00), // YELLOW
            0xF => (0xFF, 0xFF, 0xFF), // WHITE
            _ => (0x00, 0x00, 0x00)
        };

        var ratio = info.Char.UnicodeChar switch
        {
            PIXEL_SOLID => 0f,
            PIXEL_THREEQUARTERS => 0.25f,
            PIXEL_HALF => 0.5f,
            PIXEL_QUARTER => 0.5f,
            _ => 0f
        };

        var r = (int)(fg.r * (1f - ratio) + bg.r * ratio);
        var g = (int)(fg.g * (1f - ratio) + bg.g * ratio);
        var b = (int)(fg.b * (1f - ratio) + bg.b * ratio);

        return (r, g, b);
    }
}
