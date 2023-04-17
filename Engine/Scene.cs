namespace GptAdventure.Engine;

using GptAdventure.Math;

public class Scene
{
    private static CharInfo[] _debug_charinfos;

    public readonly HashSet<Obj3D> Objects = new HashSet<Obj3D>();
    public Camera Camera;
    public Vec4d LightDirection = new Vec4d(-.25f, 1f, -.66f);

    protected RenderBuffer Renderer;
    private Queue<QueuedTriangle> _trianglesToRaster = new Queue<QueuedTriangle>();
    protected Game Game;

    static Scene()
    {
        var fgAttrs = new[]
        {
            RenderBuffer.FG_DARK_BLUE,
            RenderBuffer.FG_DARK_GREEN,
            RenderBuffer.FG_DARK_CYAN,
            RenderBuffer.FG_DARK_RED,
            RenderBuffer.FG_DARK_MAGENTA,
            RenderBuffer.FG_DARK_YELLOW,
            RenderBuffer.FG_BLUE,
            RenderBuffer.FG_GREEN,
            RenderBuffer.FG_CYAN,
            RenderBuffer.FG_RED,
            RenderBuffer.FG_MAGENTA,
            RenderBuffer.FG_YELLOW
        };
        var bgAttrs = new[]
        {
            RenderBuffer.BG_DARK_BLUE,
            RenderBuffer.BG_DARK_GREEN,
            RenderBuffer.BG_DARK_CYAN,
            RenderBuffer.BG_DARK_RED,
            RenderBuffer.BG_DARK_MAGENTA,
            RenderBuffer.BG_DARK_YELLOW,
            RenderBuffer.BG_BLUE,
            RenderBuffer.BG_GREEN,
            RenderBuffer.BG_CYAN,
            RenderBuffer.BG_RED,
            RenderBuffer.BG_MAGENTA,
            RenderBuffer.BG_YELLOW
        };

        _debug_charinfos = new CharInfo[fgAttrs.Length + fgAttrs.Length * bgAttrs.Length];
        for (int i = 0; i < fgAttrs.Length; i++)
            _debug_charinfos[i] = new CharInfo(RenderBuffer.PIXEL_SOLID, fgAttrs[i]);

        for (int i = 0; i < fgAttrs.Length; i++)
        {
            for (int j = 0; j < bgAttrs.Length; j++)
            {
                var chInfo = new CharInfo(RenderBuffer.PIXEL_HALF, (ushort)(fgAttrs[i] | bgAttrs[j]));
                _debug_charinfos[fgAttrs.Length + i * fgAttrs.Length + j] = chInfo;
            }
        }
    }

    public Scene(Game game)
    {
        Game = game;
        Renderer = game.Renderer;

        float fov = 70f / 180f * MathF.PI;
        float aspectRatio = (float)Renderer.Height / (float)Renderer.Width;
        float near = .1f;
        float far = 1000f;

        Camera = new Camera(fov, aspectRatio, near, far);
    }

    public virtual void AddObject(Obj3D obj)
        => Objects.Add(obj);

    public virtual void RemoveObject(Obj3D obj)
        => Objects.Remove(obj);

    public virtual void Tick(float time, float secondsDiff)
    {
        foreach (var obj in Objects)
            obj.Tick(time, secondsDiff);
    }

    public virtual void Render()
    {
        _trianglesToRaster.Clear();

        var viewTransform = Camera.GetViewTransform();
        foreach (var obj in Objects)
        {
            if (!obj.ShouldRender())
                continue;

            var triangles = obj.GetTransformedGeometry(Mat4x4.Identity);
            foreach (var triangle in triangles)
            {
                var line1 = triangle[1].Position - triangle[0].Position;
                var line2 = triangle[2].Position - triangle[0].Position;
                var normal = line1.Cross(line2).Normalize();
                var cameraRay = triangle[0].Position - Camera.Position;
                if (normal.Dot(cameraRay) >= 0)
                    continue;

                var light = LightDirection.Dot(normal);
                if (light > 1f) light = 1f;
                if (light < .1f) light = .1f;

                var triangleViewed = triangle * viewTransform;
                var clipped = new Triangle[2];
                var clippedCount = triangleViewed.ClipAgainstPlane(new(0f, 0f, 0.1f), new(0f, 0f, 1f), out clipped[0], out clipped[1]);

                for (int j = 0; j < clippedCount; j++)
                {
                    var triangleProjected = clipped[j] * Camera.ProjectionTransform;

                    var tc1 = triangleProjected.TC1 / triangleProjected.P1.W;
                    var tc2 = triangleProjected.TC2 / triangleProjected.P2.W;
                    var tc3 = triangleProjected.TC3 / triangleProjected.P3.W;

                    var p1 = triangleProjected.P1.HomogeneousNormalize();
                    var p2 = triangleProjected.P2.HomogeneousNormalize();
                    var p3 = triangleProjected.P3.HomogeneousNormalize();

                    var w1 = 1f / p1.W;
                    var w2 = 1f / p2.W;
                    var w3 = 1f / p3.W;

                    triangleProjected = new Triangle
                    (
                        new Vec4d(p1.X, p1.Y, p1.Z, w1), tc1,
                        new Vec4d(p2.X, p2.Y, p2.Z, w2), tc2,
                        new Vec4d(p3.X, p3.Y, p3.Z, w3), tc3);
                    triangleProjected *= new Vec4d(-1f, -1f, 1f);
                    triangleProjected += new Vec4d(1f, 1f);
                    triangleProjected *= new Vec4d(.5f * Renderer.Width, .5f * Renderer.Height, 1f);

                    var charInfo = Game.Inputs.Is3Pressed ? _debug_charinfos[obj.Id % _debug_charinfos.Length] : RenderBuffer.GetCharInfoFromLum(light);
                    _trianglesToRaster.Enqueue(new(triangleProjected, light, charInfo, obj));
                }
            }
        }

        while (_trianglesToRaster.Any())
        {
            var triangleToRaster = _trianglesToRaster.Dequeue();

            var clipped = new Triangle[2];
            var trianglesQueue = new Queue<QueuedTriangle>();

            trianglesQueue.Enqueue(triangleToRaster);
            int newTriangles = 1;

            for (int i = 0; i < 4; i++)
            {
                int trisToAdd = 0;
                while (newTriangles > 0)
                {
                    var queuedTriangle = trianglesQueue.Dequeue();
                    var test = queuedTriangle.Triangle;
                    newTriangles--;

                    switch (i)
                    {
                        case 0: trisToAdd = test.ClipAgainstPlane(new(0f, 0f, 0f), new(0f, 1f, 0f), out clipped[0], out clipped[1]); break;
                        case 1: trisToAdd = test.ClipAgainstPlane(new(0f, Renderer.Height - 1f, 0f), new(0f, -1f, 0f), out clipped[0], out clipped[1]); break;
                        case 2: trisToAdd = test.ClipAgainstPlane(new(0f, 0f, 0f), new(1f, 0f, 0f), out clipped[0], out clipped[1]); break;
                        case 3: trisToAdd = test.ClipAgainstPlane(new(Renderer.Width - 1, 0f, 0f), new(-1, 0f, 0f), out clipped[0], out clipped[1]); break;
                    }

                    for (int j = 0; j < trisToAdd; j++)
                        trianglesQueue.Enqueue(new(clipped[j], queuedTriangle));
                }

                newTriangles = trianglesQueue.Count;
            }

            foreach (var queuedTriangle in trianglesQueue)
            {
                if (Game.Inputs.Is5Pressed)
                    Renderer.DrawTriangle(queuedTriangle);
                else
                    Renderer.FillTriangle(queuedTriangle, !Game.Inputs.Is3Pressed && queuedTriangle.Obj.Texture is not null);
            }
        }
    }
}
