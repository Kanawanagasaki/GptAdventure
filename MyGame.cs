namespace GptAdventure;

using GptAdventure.Engine;
using GptAdventure.Math;

public class MyGame : Game
{
    private Texture _water = Texture.FromFile($"textures/water.png");
    private Texture _grassBlockTop = Texture.FromFile($"textures/grass_block_top.png");
    private Texture _stoneBricks = Texture.FromFile($"textures/stone_bricks.png");
    private Texture _sprucePlanks = Texture.FromFile($"textures/spruce_planks.png");
    private Texture _door = Texture.FromFile($"textures/door.png");
    private Texture _tree = Texture.FromFile($"textures/tree.png");
    private Texture _man = Texture.FromFile($"textures/man.png");
    private Texture _spear = Texture.FromFile($"textures/spear.png");

    public MyGame(GptJ gpt) : base(512, 224, 4, 4)
    {
        Title = "GptAdventure";
        Scene = new MyScene(this, gpt);

        if (!File.Exists("map.obj"))
            throw new FileNotFoundException("Map file not found", "map.obj");

        var meshes = ParseObj(File.ReadAllText("map.obj"));
        foreach (var mesh in meshes)
        {
            Obj3D obj;
            if (mesh.Name == "river")
                obj = new RiverObj(mesh, _water);
            else if (mesh.Name == "awhat")
                obj = new AWhat(mesh);
            else
                obj = new Obj3D { Geometry = mesh };

            if (obj.Geometry?.Name == "terrain")
                obj.Texture = _grassBlockTop;
            else if (obj.Geometry?.Name == "castle")
                obj.Texture = _stoneBricks;
            else if (obj.Geometry?.Name == "bridge")
                obj.Texture = _sprucePlanks;
            else if (obj.Geometry?.Name == "door1" || obj.Geometry?.Name == "door2")
                obj.Texture = _door;
            else if (obj.Geometry?.Name?.StartsWith("tree") ?? false)
                obj.Texture = _tree;
            else if (obj.Geometry?.Name == "man")
                obj.Texture = _man;
            else if (obj.Geometry?.Name == "spear")
                obj.Texture = _spear;
            else if (obj.Geometry?.Name == "sword")
                obj.Texture = _spear;

            Scene.AddObject(obj);
        }
    }

    private static List<Mesh> ParseObj(string obj)
    {
        var ret = new List<Mesh>();

        bool isFirst = true;

        var name = "";
        var vectors = new List<Vec4d>();
        var normals = new List<Vec4d>();
        var textureCoords = new List<Vec2d>();
        var triangles = new List<Triangle>();

        var lines = obj.Replace("\r", "").Split("\n");
        foreach (var line in lines)
        {
            var words = line.Split(" ");
            switch (words[0])
            {
                case "o":
                    if (isFirst)
                        isFirst = false;
                    else
                    {
                        var mesh = new Mesh(triangles.ToArray()) { Name = name };
                        ret.Add(mesh);
                        triangles.Clear();
                    }

                    name = words[1];
                    break;
                case "v":
                    vectors.Add(new Vec4d(float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3])));
                    break;
                case "vn":
                    normals.Add(new Vec4d(float.Parse(words[1]), float.Parse(words[2]), float.Parse(words[3])));
                    break;
                case "vt":
                    textureCoords.Add(new Vec2d(float.Parse(words[1]), float.Parse(words[2])));
                    break;
                case "f":

                    var a = words[1].Split("/");
                    var b = words[2].Split("/");
                    var c = words[3].Split("/");

                    var v1 = int.Parse(a[0]) - 1;
                    var v2 = int.Parse(b[0]) - 1;
                    var v3 = int.Parse(c[0]) - 1;

                    var vt1 = int.Parse(a[1]) - 1;
                    var vt2 = int.Parse(b[1]) - 1;
                    var vt3 = int.Parse(c[1]) - 1;

                    var vn1 = int.Parse(a[2]) - 1;
                    var vn2 = int.Parse(b[2]) - 1;
                    var vn3 = int.Parse(c[2]) - 1;

                    var triangle = new Triangle
                    (
                        vectors[v1], textureCoords[vt1],
                        vectors[v2], textureCoords[vt2],
                        vectors[v3], textureCoords[vt3]
                    );

                    triangles.Add(triangle);

                    break;

            }
        }

        {
            var mesh = new Mesh(triangles.ToArray()) { Name = name };
            ret.Add(mesh);
        }

        return ret;
    }
}