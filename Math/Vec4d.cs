namespace GptAdventure.Math;

public struct Vec4d
{
    public static readonly Vec4d Zero = new Vec4d(0, 0, 0);

    public static readonly Vec4d RIGHT = new Vec4d(1, 0, 0);
    public static readonly Vec4d UP = new Vec4d(0, 1, 0);
    public static readonly Vec4d FORWARD = new Vec4d(0, 0, 1);
    public static readonly Vec4d LEFT = new Vec4d(-1, 0, 0);
    public static readonly Vec4d DOWN = new Vec4d(0, -1, 0);
    public static readonly Vec4d BACKWARD = new Vec4d(0, 0, -1);

    public readonly float X;
    public readonly float Y;
    public readonly float Z;
    public readonly float W;

    public float LengthSq => X * X + Y * Y + Z * Z;
    public float Length => MathF.Sqrt(LengthSq);

    public Vec4d(float x = 0, float y = 0, float z = 0, float w = 1)
        => (X, Y, Z, W) = (x, y, z, w);

    public Vec4d Normalize()
    {
        var len = Length;
        return new(X / len, Y / len, Z / len, W);
    }

    public float Dot(Vec4d vec)
        => X * vec.X + Y * vec.Y + Z * vec.Z;

    public Vec4d Cross(Vec4d vec)
        => new Vec4d
        (
            Y * vec.Z - Z * vec.Y,
            Z * vec.X - X * vec.Z,
            X * vec.Y - Y * vec.X,
            W
        );

    public Vec4d HomogeneousNormalize()
        => new Vec4d(X / W, Y / W, Z / W, W);

    public static Vec4d operator *(Vec4d v, Mat4x4 m)
        => new Vec4d
        (
            v.X * m[0, 0] + v.Y * m[1, 0] + v.Z * m[2, 0] + v.W * m[3, 0],
            v.X * m[0, 1] + v.Y * m[1, 1] + v.Z * m[2, 1] + v.W * m[3, 1],
            v.X * m[0, 2] + v.Y * m[1, 2] + v.Z * m[2, 2] + v.W * m[3, 2],
            v.X * m[0, 3] + v.Y * m[1, 3] + v.Z * m[2, 3] + v.W * m[3, 3]
        );

    public static Vec4d operator +(Vec4d vec1, Vec4d vec2)
        => new Vec4d(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z, vec1.W);

    public static Vec4d operator -(Vec4d vec1, Vec4d vec2)
        => new Vec4d(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z, vec1.W);

    public static Vec4d operator *(Vec4d vec1, Vec4d vec2)
        => new Vec4d(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z, vec1.W);

    public static Vec4d operator /(Vec4d vec1, Vec4d vec2)
        => new Vec4d(vec1.X / vec2.X, vec1.Y / vec2.Y, vec1.Z / vec2.Z, vec1.W);

    public static Vec4d operator *(Vec4d vec, float scale)
        => new Vec4d(vec.X * scale, vec.Y * scale, vec.Z * scale, vec.W);

    public static Vec4d operator /(Vec4d vec, float scale)
        => new Vec4d(vec.X / scale, vec.Y / scale, vec.Z / scale, vec.W);

    public override string ToString()
        => $"{{ X: {X}, Y: {Y}, Z: {Z}, W: {W} }}";

    public static Vec4d IntersectPlane(Vec4d planePoint, Vec4d planeNormal, Vec4d vecStart, Vec4d vecEnd, out float t)
    {
        planeNormal = planeNormal.Normalize();
        float d = -planeNormal.Dot(planePoint);
        float ad = vecStart.Dot(planeNormal);
        float bd = vecEnd.Dot(planeNormal);
        t = (-d - ad) / (bd - ad);

        var vecStartToEnd = vecEnd - vecStart;
        var vec = vecStart + vecStartToEnd * t;

        var w = t * (vecEnd.W - vecStart.W) + vecStart.W;

        return new Vec4d(vec.X, vec.Y, vec.Z, w);
    }
}
