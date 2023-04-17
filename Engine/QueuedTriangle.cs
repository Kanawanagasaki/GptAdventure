namespace GptAdventure.Engine;

using GptAdventure.Math;

public struct QueuedTriangle
{
    public Triangle Triangle;
    public float Light;
    public CharInfo ChInfo;
    public Obj3D Obj;

    public QueuedTriangle(Triangle triangle, float light, CharInfo chInfo, Obj3D obj)
    {
        Triangle = triangle;
        Light = light;
        ChInfo = chInfo;
        Obj = obj;
    }

    public QueuedTriangle(Triangle newTriangle, QueuedTriangle copyFrom)
    {
        Triangle = newTriangle;
        Light = copyFrom.Light;
        ChInfo = copyFrom.ChInfo;
        Obj = copyFrom.Obj;
    }
}
