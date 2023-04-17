namespace GptAdventure.Engine;

using GptAdventure.Math;

public class Camera
{
    public Vec4d Position = Vec4d.Zero;

    public Vec4d Rotation = Vec4d.Zero;

    public float Fov { get; private set; }
    public float AspectRatio { get; private set; }
    public float Near { get; private set; }
    public float Far { get; private set; }
    public Mat4x4 ProjectionTransform { get; private set; }

    public Vec4d LookDir => Vec4d.FORWARD * Mat4x4.RotateY(Rotation.Y) * Mat4x4.RotateX(Rotation.X) * Mat4x4.RotateZ(Rotation.Z);

    public Camera(float fov, float aspectRatio, float near, float far)
    {
        Fov = fov;
        AspectRatio = aspectRatio;
        Near = near;
        Far = far;
        ProjectionTransform = Mat4x4.Frustum(fov, aspectRatio, near, far);
    }

    public Mat4x4 GetViewTransform()
        => Mat4x4.PointAt(Position, Position + LookDir, Vec4d.UP).QuickInverse();
}
