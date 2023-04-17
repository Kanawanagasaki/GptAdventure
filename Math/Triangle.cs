namespace GptAdventure.Math;

using System.Collections;

public struct Triangle : IEnumerable<(Vec4d Position, Vec2d TextureCoord)>
{
    public readonly Vec4d P1;
    public readonly Vec2d TC1;
    public readonly Vec4d P2;
    public readonly Vec2d TC2;
    public readonly Vec4d P3;
    public readonly Vec2d TC3;

    public (Vec4d Position, Vec2d TextureCoord) this[int index] => index switch
    {
        0 => (P1, TC1),
        1 => (P2, TC2),
        2 => (P3, TC3),
        _ => throw new IndexOutOfRangeException()
    };

    public Triangle(Vec4d p1, Vec2d tc1, Vec4d p2, Vec2d tc2, Vec4d p3, Vec2d tc3)
        => (P1, TC1, P2, TC2, P3, TC3) = (p1, tc1, p2, tc2, p3, tc3);

    public int ClipAgainstPlane(Vec4d planePoint, Vec4d planeNormal, out Triangle tri1, out Triangle tri2)
    {
        tri1 = default;
        tri2 = default;

        var dist = (Vec4d point) => planeNormal.X * point.X + planeNormal.Y * point.Y + planeNormal.Z * point.Z - planeNormal.Dot(planePoint);

        planeNormal = planeNormal.Normalize();

        var insidePoints = new Vec4d[3];
        int insidePointsCount = 0;
        var insideTextureCoords = new Vec2d[3];
        int insideTextureCoordsCount = 0;

        var outsidePoints = new Vec4d[3];
        int outsidePointsCount = 0;
        var outsideTextureCoords = new Vec2d[3];
        int outsideTextureCoordsCount = 0;

        var ds = new[] { dist(P1), dist(P2), dist(P3) };

        for (int i = 0; i < ds.Length; i++)
        {
            if (ds[i] >= 0)
            {
                insidePoints[insidePointsCount++] = this[i].Position;
                insideTextureCoords[insideTextureCoordsCount++] = this[i].TextureCoord;
            }
            else
            {
                outsidePoints[outsidePointsCount++] = this[i].Position;
                outsideTextureCoords[outsideTextureCoordsCount++] = this[i].TextureCoord;
            }
        }

        if (insidePointsCount == 0)
            return 0;

        if (insidePointsCount == 3)
        {
            tri1 = this;
            return 1;
        }

        if (insidePointsCount == 1 && outsidePointsCount == 2)
        {
            var p2 = Vec4d.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0], out var t2);
            var tc2 = insideTextureCoords[0] + (outsideTextureCoords[0] - insideTextureCoords[0]) * t2;

            var p3 = Vec4d.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[1], out var t3);
            var tc3 = insideTextureCoords[0] + (outsideTextureCoords[1] - insideTextureCoords[0]) * t3;

            tri1 = new Triangle(insidePoints[0], insideTextureCoords[0], p2, tc2, p3, tc3);

            return 1;
        }

        if (insidePointsCount == 2 && outsidePointsCount == 1)
        {
            var p1 = Vec4d.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0], out var t1);
            var tc1 = insideTextureCoords[0] + (outsideTextureCoords[0] - insideTextureCoords[0]) * t1;

            tri1 = new Triangle(insidePoints[0], insideTextureCoords[0], insidePoints[1], insideTextureCoords[1], p1, tc1);

            var p2 = Vec4d.IntersectPlane(planePoint, planeNormal, insidePoints[1], outsidePoints[0], out var t2);
            var tc2 = insideTextureCoords[1] + (outsideTextureCoords[0] - insideTextureCoords[1]) * t2;

            tri2 = new Triangle(insidePoints[1], insideTextureCoords[1], p1, tc1, p2, tc2);

            return 2;
        }

        return 0;
    }

    public bool IsPointInside(Vec4d point)
    {
        var a = P1 - point;
        var b = P2 - point;
        var c = P3 - point;

        var u = b.Cross(c);
        var v = c.Cross(a);
        var w = a.Cross(b);

        return u.Dot(v) >= 0f && u.Dot(w) >= 0f;
    }

    public IEnumerator<(Vec4d Position, Vec2d TextureCoord)> GetEnumerator()
    {
        for (int i = 0; i < 3; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public static Triangle operator *(Triangle tri, Mat4x4 mat)
        => new Triangle(tri.P1 * mat, tri.TC1, tri.P2 * mat, tri.TC2, tri.P3 * mat, tri.TC3);

    public static Triangle operator +(Triangle tri, Vec4d vec)
        => new Triangle(tri.P1 + vec, tri.TC1, tri.P2 + vec, tri.TC2, tri.P3 + vec, tri.TC3);

    public static Triangle operator -(Triangle tri, Vec4d vec)
        => new Triangle(tri.P1 - vec, tri.TC1, tri.P2 - vec, tri.TC2, tri.P3 - vec, tri.TC3);

    public static Triangle operator *(Triangle tri, Vec4d vec)
        => new Triangle(tri.P1 * vec, tri.TC1, tri.P2 * vec, tri.TC2, tri.P3 * vec, tri.TC3);

    public static Triangle operator /(Triangle tri, Vec4d vec)
        => new Triangle(tri.P1 / vec, tri.TC1, tri.P2 / vec, tri.TC2, tri.P3 / vec, tri.TC3);

    public static Triangle operator *(Triangle tri, float scale)
        => new Triangle(tri.P1 * scale, tri.TC1, tri.P2 * scale, tri.TC2, tri.P3 * scale, tri.TC3);

    public static Triangle operator /(Triangle tri, float scale)
        => new Triangle(tri.P1 / scale, tri.TC1, tri.P2 / scale, tri.TC2, tri.P3 / scale, tri.TC3);

    public override string ToString()
        => $"(\n\t{P1} {TC1},\n\t{P2} {TC2},\n\t{P3} {TC3}\n)";
}
