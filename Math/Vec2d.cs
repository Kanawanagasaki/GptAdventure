namespace GptAdventure.Math;

public struct Vec2d
{

    public readonly float U;
    public readonly float V;

    public float LengthSq => U * U + V * V;
    public float Length => MathF.Sqrt(LengthSq);

    public Vec2d(float x = 0, float y = 0)
        => (U, V) = (x, y);

    public Vec2d Normalize()
    {
        var len = Length;
        return new(U / len, V / len);
    }

    public float Dot(Vec2d vec)
        => U * vec.U + V * vec.V;

    public static Vec2d operator +(Vec2d vec1, Vec2d vec2)
        => new Vec2d(vec1.U + vec2.U, vec1.V + vec2.V);

    public static Vec2d operator -(Vec2d vec1, Vec2d vec2)
        => new Vec2d(vec1.U - vec2.U, vec1.V - vec2.V);

    public static Vec2d operator *(Vec2d vec1, Vec2d vec2)
        => new Vec2d(vec1.U * vec2.U, vec1.V * vec2.V);

    public static Vec2d operator /(Vec2d vec1, Vec2d vec2)
        => new Vec2d(vec1.U / vec2.U, vec1.V / vec2.V);

    public static Vec2d operator *(Vec2d vec, float scale)
        => new Vec2d(vec.U * scale, vec.V * scale);

    public static Vec2d operator /(Vec2d vec, float scale)
        => new Vec2d(vec.U / scale, vec.V / scale);

    public override string ToString()
        => $"{{ X: {U}, Y: {V} }}";
}
