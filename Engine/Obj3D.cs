namespace GptAdventure.Engine;

using GptAdventure.Math;

public class Obj3D
{
    private static int _next_id = 1;
    public readonly int Id = ++_next_id;

    public Vec4d Position = Vec4d.Zero;
    public Vec4d Rotation = Vec4d.Zero;
    public Mesh? Geometry { get; set; } = null;
    public Texture? Texture { get; set; } = null;
    public Obj3D[] Children = Array.Empty<Obj3D>();

    public virtual bool ShouldRender() => true;

    public virtual void Tick(float time, float secondsDiff) { }

    public Triangle[] GetTransformedGeometry(Mat4x4 parentTransform)
    {
        var ret = Geometry is null ? Array.Empty<Triangle>() : new Triangle[Geometry.Triangles.Length];

        var transform = parentTransform * GetTransform();
        for (int i = 0; i < ret.Length; i++)
            ret[i] = Geometry!.Triangles[i] * transform;

        foreach (var child in Children)
        {
            var childTriangles = child.GetTransformedGeometry(Mat4x4.Identity);
            ret = ret.Concat(childTriangles).ToArray();
        }

        return ret;
    }

    public Mat4x4 GetTransform()
    {
        var modelMat = Mat4x4.Identity;
        if (Rotation.Y != 0)
            modelMat *= Mat4x4.RotateY(Rotation.Y);
        if (Rotation.X != 0)
            modelMat *= Mat4x4.RotateX(Rotation.X);
        if (Rotation.Z != 0)
            modelMat *= Mat4x4.RotateZ(Rotation.Z);
        modelMat *= Mat4x4.Translate(Position);
        return modelMat;
    }
}
