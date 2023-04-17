namespace GptAdventure;

using GptAdventure.Engine;

public class RiverObj : Obj3D
{
    private Texture[] _animation;

    public RiverObj(Mesh mesh, Texture texture)
    {
        this.Geometry = mesh;
        _animation = new Texture[texture.Height / 32];

        var width = texture.Width;
        var height = texture.Height / _animation.Length;

        for (int i = 0; i < _animation.Length; i++)
        {
            var pixels = new CharInfo[height, width];
            for (int iy = 0; iy < height; iy++)
                for (int ix = 0; ix < width; ix++)
                    pixels[iy, ix] = texture.Pixels[iy + i * height, ix];
            _animation[i] = new Texture(pixels);
        }
    }

    public override void Tick(float time, float secondsDiff)
    {
        var index = (int)(time * 10f) % _animation.Length;
        Texture = _animation[index];

        base.Tick(time, secondsDiff);
    }
}