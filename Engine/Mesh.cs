namespace GptAdventure.Engine;

using GptAdventure.Math;

public class Mesh
{
    public string Name { get; init; } = string.Empty;
    public readonly Triangle[] Triangles;

    public Mesh(Triangle[] triangles)
        => Triangles = triangles;

    public void Normalize()
    {
        float maxX, minX, maxY, minY, maxZ, minZ;
        maxX = maxY = maxZ = float.MinValue;
        minX = minY = minZ = float.MaxValue;
        foreach (var tri in Triangles)
        {
            foreach (var vert in tri)
            {
                if (vert.Position.X < minX) minX = vert.Position.X;
                if (maxX < vert.Position.X) maxX = vert.Position.X;
                if (vert.Position.Y < minY) minY = vert.Position.Y;
                if (maxY < vert.Position.Y) maxY = vert.Position.Y;
                if (vert.Position.Z < minZ) minZ = vert.Position.Z;
                if (maxZ < vert.Position.Z) maxZ = vert.Position.Z;
            }
        }

        float diffX = (maxX - minX) / -2 - minX;
        float diffY = (maxY - minY) / -2 - minY;
        float diffZ = (maxZ - minZ) / -2 - minZ;

        for (int i = 0; i < Triangles.Length; i++)
            Triangles[i] += new Vec4d(diffX, diffY, diffZ);
    }

    public Mesh Clone(string? name = null)
    {
        var triangles = new Triangle[Triangles.Length];
        for (int i = 0; i < Triangles.Length; i++)
            triangles[i] = Triangles[i];
        return new Mesh(triangles) { Name = name ?? Guid.NewGuid().ToString("N") };
    }
}