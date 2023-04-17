namespace GptAdventure;

using System.Runtime.InteropServices;

public static class Native
{
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern nint CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteConsoleOutput(
        nint hConsoleOutput,
        CharInfo[] lpBuffer,
        ConsoleCoord dwBufferSize,
        ConsoleCoord dwBufferCoord,
        ref SmallRect lpWriteRegion);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleOutputCP(uint wCodePageID);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleCP(uint wCodePageID);

    public const int STD_OUTPUT_HANDLE = -11;
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nint GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleWindowInfo(
        nint hConsoleOutput,
        bool bAbsolute,
        [In] ref SmallRect lpConsoleWindow);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleScreenBufferSize(nint hConsoleOutput, ConsoleCoord dwSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleActiveScreenBuffer(nint hConsole);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int SetCurrentConsoleFontEx(
        nint ConsoleOutput,
        bool MaximumWindow,
        ref ConsoleFontInfo ConsoleCurrentFontEx);

    [Flags]
    public enum ConsoleModes : uint
    {
        ENABLE_PROCESSED_INPUT = 0x0001,
        ENABLE_LINE_INPUT = 0x0002,
        ENABLE_ECHO_INPUT = 0x0004,
        ENABLE_WINDOW_INPUT = 0x0008,
        ENABLE_MOUSE_INPUT = 0x0010,
        ENABLE_INSERT_MODE = 0x0020,
        ENABLE_QUICK_EDIT_MODE = 0x0040,
        ENABLE_EXTENDED_FLAGS = 0x0080,
        ENABLE_AUTO_POSITION = 0x0100,

        ENABLE_PROCESSED_OUTPUT = 0x0001,
        ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
        ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
        DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
        ENABLE_LVB_GRID_WORLDWIDE = 0x0010
    }
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleMode(IntPtr hConsoleHandle, ConsoleModes dwMode);

    [DllImport("user32.dll")]
    public static extern short GetKeyState(int virtKey);
    [DllImport("user32.dll")]
    public static extern int SetCursorPos(int x, int y);
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out Point point);
    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleTitle(string title);
}

#region Structures
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct ConsoleFontInfo
{
    public uint cbSize;
    public uint nFont;
    public ConsoleCoord dwFontSize;
    public int FontFamily;
    public int FontWeight;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string FaceName;
}

[StructLayout(LayoutKind.Sequential)]
public struct ConsoleCoord
{
    public short X;
    public short Y;

    public ConsoleCoord(short x, short y)
    {
        X = x;
        Y = y;
    }
};

[StructLayout(LayoutKind.Sequential)]
public struct CharUnion
{
    public ushort UnicodeChar;

    public CharUnion(char ch)
    {
        UnicodeChar = ch;
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct CharInfo
{
    [FieldOffset(0)] public CharUnion Char;
    [FieldOffset(2)] public ushort Attributes;

    public CharInfo()
    {
        Char = new CharUnion(' ');
        Attributes = 0;
    }

    public CharInfo(char ch)
    {
        Char = new CharUnion(ch);
        Attributes = 0;
    }

    public CharInfo(char ch, ushort attr)
    {
        Char = new CharUnion(ch);
        Attributes = attr;
    }

    public CharInfo(CharUnion ch)
    {
        Char = ch;
        Attributes = 0;
    }

    public CharInfo(CharUnion ch, ushort attr)
    {
        Char = ch;
        Attributes = attr;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct SmallRect
{
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;
}

[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public int X;
    public int Y;
}
#endregion