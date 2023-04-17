namespace GptAdventure;

using GptAdventure.Engine;

public class AWhat : Obj3D
{
    private Texture[] _animation;

    public AWhat(Mesh mesh)
    {
        Geometry = mesh;

        using var stream = File.OpenRead("textures/awhat.bin");
        using var reader = new BinaryReader(stream);
        var framesAmount = reader.ReadInt32();
        _animation = new Texture[framesAmount];
        for (int i = 0; i < framesAmount; i++)
        {
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var pixels = new CharInfo[height, width];
            for (int iy = 0; iy < height; iy++)
            {
                for (int ix = 0; ix < width; ix++)
                {
                    var ch = reader.ReadUInt16();
                    var attr = reader.ReadUInt16();
                    pixels[iy, ix] = new CharInfo((char)ch, attr);
                }
            }
            _animation[i] = new Texture(pixels);
        }
    }

    public override void Tick(float time, float secondsDiff)
    {
        Texture = _animation[(int)(time * 15f) % _animation.Length];
        base.Tick(time, secondsDiff);
    }
}
