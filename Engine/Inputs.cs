namespace GptAdventure.Engine;

public class Inputs
{
    public bool IsWPressed { get; private set; } = false;
    public bool IsAPressed { get; private set; } = false;
    public bool IsSPressed { get; private set; } = false;
    public bool IsDPressed { get; private set; } = false;

    public bool WasEPressedLastTick { get; private set; } = false;
    public bool IsEPressed { get; private set; } = false;
    public bool IsEJustPressed => !WasEPressedLastTick && IsEPressed;

    public bool Was1PressedLastTick { get; private set; } = false;
    public bool Is1Pressed { get; private set; } = false;
    public bool Is1JustPressed => !Was1PressedLastTick && Is1Pressed;

    public bool Was2PressedLastTick { get; private set; } = false;
    public bool Is2Pressed { get; private set; } = false;
    public bool Is2JustPressed => !Was2PressedLastTick && Is2Pressed;

    public bool Is3Pressed { get; private set; } = false;
    public bool Is4Pressed { get; private set; } = false;
    public bool Is5Pressed { get; private set; } = false;
    public bool Is6Pressed { get; private set; } = false;
    public bool Is7Pressed { get; private set; } = false;
    public bool Is8Pressed { get; private set; } = false;
    public bool Is9Pressed { get; private set; } = false;
    public bool Is0Pressed { get; private set; } = false;

    public int MouseX { get; private set; } = 0;
    public int MouseY { get; private set; } = 0;

    public void Init()
    {
        Native.SetCursorPos(320, 0);
    }

    public void Tick()
    {
        Was1PressedLastTick = Is1Pressed;
        Was2PressedLastTick = Is2Pressed;

        WasEPressedLastTick = IsEPressed;

        IsEPressed = IsKeyPressed(69);

        IsWPressed = IsKeyPressed(87);
        IsAPressed = IsKeyPressed(65);
        IsSPressed = IsKeyPressed(83);
        IsDPressed = IsKeyPressed(68);
        Is1Pressed = IsKeyPressed(49);
        Is2Pressed = IsKeyPressed(50);
        Is3Pressed = IsKeyPressed(51);
        Is4Pressed = IsKeyPressed(52);
        Is5Pressed = IsKeyPressed(53);
        Is6Pressed = IsKeyPressed(54);
        Is7Pressed = IsKeyPressed(55);
        Is8Pressed = IsKeyPressed(56);
        Is9Pressed = IsKeyPressed(57);
        Is0Pressed = IsKeyPressed(48);

        Native.GetCursorPos(out var point);
        MouseX = point.X - 512;
        MouseY = point.Y - 0;
        Native.SetCursorPos(512, 0);
    }

    private bool IsKeyPressed(int virtKey)
        => Native.GetKeyState(virtKey) switch
        {
            0 => false,
            1 => false,
            _ => true
        };
}
