namespace GptAdventure;

using GptAdventure.Engine;
using GptAdventure.Math;

public class MyScene : Scene
{
    private Mesh? _navigation = null;

    private GptJ _gpt;
    private AzureTts _tts;
    private AzureRecognizer _recognizer;

    private EGuardState _guardState;
    private Vec4d _guardPos = new Vec4d(49.5f, 2.2f, 45.8f);
    private List<(string text, bool isGuard)> _conversation = new();
    private Vec4d _swordPos = new Vec4d(24.87f, 5.7f, -15.67f);
    private Obj3D? _sword = null;
    private bool _isSwordWithGuard = false;
    private bool _shouldOpenTheDoors = false;

    private Obj3D? _door1 = null;
    private Obj3D? _door2 = null;

    public MyScene(Game game, GptJ gpt) : base(game)
    {
        _recognizer = new AzureRecognizer();
        _recognizer.OnNewResult += OnNewRecognition;

        _gpt = gpt;
        _gpt.OnResult += OnNewThoughts;

        _tts = new AzureTts("en-US-DavisNeural");
        _tts.OnFinishSpeacking += OnFinishSpeacking;
    }

    public override void AddObject(Obj3D obj)
    {
        if (obj.Geometry?.Name == "sword")
            _sword = obj;

        if (obj.Geometry?.Name == "door1")
        {
            obj.Position = new Vec4d(51.0858f, 0f, 48.9142f);
            for (int i = 0; i < obj.Geometry.Triangles.Length; i++)
                obj.Geometry.Triangles[i] -= obj.Position;
            _door1 = obj;
        }

        if (obj.Geometry?.Name == "door2")
        {
            obj.Position = new Vec4d(48.9142f, 0f, 51.0858f);
            for (int i = 0; i < obj.Geometry.Triangles.Length; i++)
                obj.Geometry.Triangles[i] -= obj.Position;
            _door2 = obj;
        }

        if (obj.Geometry?.Name == "navigation")
            _navigation = obj.Geometry;
        else
            base.AddObject(obj);

        Camera.Position = new Vec4d(0, 10.7166f + 2f, 0);
    }

    public override void Tick(float time, float secondsDiff)
    {
        var lookDir = Camera.LookDir;
        var forward = lookDir * (8f * secondsDiff);
        var right = lookDir.Cross(Vec4d.UP).Normalize() * (8f * secondsDiff);

        if (_navigation is not null)
        {
            var cameraPosition = Camera.Position;

            if (Game.Inputs.IsWPressed)
                cameraPosition += forward;
            if (Game.Inputs.IsSPressed)
                cameraPosition -= forward;
            if (Game.Inputs.IsAPressed)
                cameraPosition -= right;
            if (Game.Inputs.IsDPressed)
                cameraPosition += right;

            if (_isSwordWithGuard || cameraPosition.X < 49 || cameraPosition.Z < 49)
            {
                foreach (var triangle in _navigation.Triangles)
                {
                    var line1 = triangle.P2 - triangle.P1;
                    var line2 = triangle.P3 - triangle.P1;
                    var normal = line1.Cross(line2).Normalize();

                    var vecStart = new Vec4d(cameraPosition.X, triangle.P1.Y + 1f, cameraPosition.Z);
                    var vecEnd = new Vec4d(cameraPosition.X, triangle.P1.Y - 1f, cameraPosition.Z);

                    var intersectionPoint = Vec4d.IntersectPlane(triangle.P1, normal, vecStart, vecEnd, out _);
                    if (triangle.IsPointInside(intersectionPoint))
                    {
                        Camera.Position = new Vec4d(cameraPosition.X, intersectionPoint.Y + 2f, cameraPosition.Z);
                        break;
                    }
                }
            }
        }

        if (_door1 is not null && _shouldOpenTheDoors && MathF.PI * -.45f < _door1.Rotation.Y)
            _door1.Rotation += new Vec4d(0, -0.5f * secondsDiff, 0f);
        if (_door2 is not null && _shouldOpenTheDoors && _door2.Rotation.Y < MathF.PI * .45f)
            _door2.Rotation += new Vec4d(0, 0.5f * secondsDiff, 0f);

        Camera.Rotation = new Vec4d(y: Camera.Rotation.Y + Game.Inputs.MouseX * secondsDiff * .25f);

        if (_guardState == EGuardState.Idle && (_guardPos - Camera.Position).Length < 7.5f)
        {
            _guardState = EGuardState.Listening;
            _recognizer.Activate();
            Logger.WriteLine("Guard is listening");
        }

        if (_guardState == EGuardState.Listening && (_guardPos - Camera.Position).Length >= 7.5f)
        {
            _guardState = EGuardState.Idle;
            _recognizer.Deactivate();
            Logger.WriteLine("Guard is idling");
            _conversation.Clear();
        }

        if (_sword is not null && (_swordPos - Camera.Position).Length < 3f && Game.Inputs.IsEJustPressed)
        {
            RemoveObject(_sword);
            _sword = null;
            _conversation.Clear();
        }

        if (_sword is null && !_isSwordWithGuard && (_guardPos - Camera.Position).Length < 7.5f && Game.Inputs.IsEJustPressed)
        {
            _isSwordWithGuard = true;
            _conversation.Clear();

            _guardState = EGuardState.Thinking;
            _recognizer.Deactivate();

            var story = $"""
                Guard standing next to castle gate.
                Traveler approaches guard.
                Guard is letting traveler to go inside the castle saying: "
                """.Replace("\r", "").Replace("\n", " ");

            _gpt.Generate(story);
        }

        base.Tick(time, secondsDiff);
    }

    public override void Render()
    {
        base.Render();

        var str = "";

        if (!_isSwordWithGuard)
            str += "Objective: Get inside the castle";

        if (_sword is not null && (_swordPos - Camera.Position).Length < 3f)
            str += "\nPress E to collect the sword";

        if (_sword is null && !_isSwordWithGuard && (_guardPos - Camera.Position).Length < 7.5f)
            str += "\nPress E to give the sword to the guard";

        if (_guardState == EGuardState.Listening)
            str += "\nGuard is listening";
        else if (_guardState != EGuardState.Idle && _guardState != EGuardState.Speaking)
            str += "\nGuard is thinking";

        Renderer.DrawText(str, RenderBuffer.FG_WHITE, 1, 1);
    }

    private void OnNewRecognition(string text)
    {
        if (_guardState != EGuardState.Listening)
        {
            _guardState = EGuardState.Idle;
            _recognizer.Deactivate();
            return;
        }

        _guardState = EGuardState.Thinking;
        _recognizer.Deactivate();

        _conversation.Add((text.Replace("\"", ""), false));
        var conversation = "";
        for (int i = System.Math.Max(0, _conversation.Count - 5); i < _conversation.Count; i++)
        {
            conversation += _conversation[i].isGuard ? "Guard said: " : "Traveler said: ";
            conversation += $"\"{_conversation[i].text}\". ";
        }

        string story;
        if (_sword is not null)
            story = $"""
                Guard standing next to castle gate.
                Guard have been ordered not to let anyone into the castle.
                Guard will let in only the one who gives guard the sword.
                Sword is in the field next to the trees.
                Traveler approaches guard.
                {conversation}
                Guard reply: "
                """.Replace("\r", "").Replace("\n", " ");
        else if (!_isSwordWithGuard)
            story = $"""
                Guard standing next to castle gate.
                Traveler with a sword approaches guard.
                Traveler must give guard a sword.
                {conversation}
                Guard reply: "
                """.Replace("\r", "").Replace("\n", " ");
        else
            story = $"""
                Guard standing next to castle gate.
                Traveler approaches guard.
                Guard will allow traveler to go inside the castle.
                {conversation}
                Guard reply: "
                """.Replace("\r", "").Replace("\n", " ");

        _gpt.Generate(story);
    }

    private void OnNewThoughts(string text)
    {
        if (_guardState != EGuardState.Thinking)
        {
            _guardState = EGuardState.Idle;
            _recognizer.Deactivate();
            return;
        }

        _guardState = EGuardState.Speaking;

        if (text.StartsWith(_gpt.LastPrompt))
            text = text.Substring(_gpt.LastPrompt.Length);

        int quoteIndex = text.IndexOf('"');
        if (quoteIndex >= 0)
            text = text.Substring(0, quoteIndex);

        _conversation.Add((text, true));
        _tts.AddTextToRead(text);
    }

    private void OnFinishSpeacking()
    {
        if (_guardState != EGuardState.Speaking)
        {
            _guardState = EGuardState.Idle;
            _recognizer.Deactivate();
            return;
        }

        _guardState = EGuardState.Listening;
        _recognizer.Activate();

        if (_isSwordWithGuard)
            _shouldOpenTheDoors = true;
    }

    private enum EGuardState
    {
        Idle, Listening, Recognizing, Thinking, Speaking
    }
}
