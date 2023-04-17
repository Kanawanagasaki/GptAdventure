namespace GptAdventure.Math;

public struct Mat4x4
{
    public static readonly Mat4x4 Identity = new Mat4x4
    {
        M00 = 1f,
        M11 = 1f,
        M22 = 1f,
        M33 = 1f
    };

    public float M00;
    public float M01;
    public float M02;
    public float M03;
    public float M10;
    public float M11;
    public float M12;
    public float M13;
    public float M20;
    public float M21;
    public float M22;
    public float M23;
    public float M30;
    public float M31;
    public float M32;
    public float M33;

    public float this[int offset]
    {
        get => offset switch
        {
            0 => M00,
            1 => M01,
            2 => M02,
            3 => M03,
            4 => M10,
            5 => M11,
            6 => M12,
            7 => M13,
            8 => M20,
            9 => M21,
            10 => M22,
            11 => M23,
            12 => M30,
            13 => M31,
            14 => M32,
            15 => M33,
            _ => throw new IndexOutOfRangeException()
        };
        set
        {
            switch (offset)
            {
                case 0: M00 = value; break;
                case 1: M01 = value; break;
                case 2: M02 = value; break;
                case 3: M03 = value; break;
                case 4: M10 = value; break;
                case 5: M11 = value; break;
                case 6: M12 = value; break;
                case 7: M13 = value; break;
                case 8: M20 = value; break;
                case 9: M21 = value; break;
                case 10: M22 = value; break;
                case 11: M23 = value; break;
                case 12: M30 = value; break;
                case 13: M31 = value; break;
                case 14: M32 = value; break;
                case 15: M33 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public float this[int row, int column]
    {
        get => (row, column) switch
        {
            (0, 0) => M00,
            (0, 1) => M01,
            (0, 2) => M02,
            (0, 3) => M03,
            (1, 0) => M10,
            (1, 1) => M11,
            (1, 2) => M12,
            (1, 3) => M13,
            (2, 0) => M20,
            (2, 1) => M21,
            (2, 2) => M22,
            (2, 3) => M23,
            (3, 0) => M30,
            (3, 1) => M31,
            (3, 2) => M32,
            (3, 3) => M33,
            _ => throw new IndexOutOfRangeException()
        };
        set
        {
            switch ((row, column))
            {
                case (0, 0): M00 = value; break;
                case (0, 1): M01 = value; break;
                case (0, 2): M02 = value; break;
                case (0, 3): M03 = value; break;
                case (1, 0): M10 = value; break;
                case (1, 1): M11 = value; break;
                case (1, 2): M12 = value; break;
                case (1, 3): M13 = value; break;
                case (2, 0): M20 = value; break;
                case (2, 1): M21 = value; break;
                case (2, 2): M22 = value; break;
                case (2, 3): M23 = value; break;
                case (3, 0): M30 = value; break;
                case (3, 1): M31 = value; break;
                case (3, 2): M32 = value; break;
                case (3, 3): M33 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public Mat4x4 QuickInverse()
        => new Mat4x4
        {
            #pragma warning disable format

            M00 = M00, M01 = M10, M02 = M20, M03 = 0f,
            M10 = M01, M11 = M11, M12 = M21, M13 = 0f,
            M20 = M02, M21 = M12, M22 = M22, M23 = 0f,

            M30 =  -(M30 * M00 + M31 * M01 + M32 * M02),
            M31 =  -(M30 * M10 + M31 * M11 + M32 * M12),
            M32 =  -(M30 * M20 + M31 * M21 + M32 * M22),

            M33 = 1f

            #pragma warning restore format
        };

    public override string ToString()
        => $$"""
            {
            {{'\t'}}{{M00:0.000}}{{'\t'}}{{M01:0.000}}{{'\t'}}{{M02:0.000}}{{'\t'}}{{M03:0.000}}
            {{'\t'}}{{M10:0.000}}{{'\t'}}{{M11:0.000}}{{'\t'}}{{M12:0.000}}{{'\t'}}{{M13:0.000}}
            {{'\t'}}{{M20:0.000}}{{'\t'}}{{M21:0.000}}{{'\t'}}{{M22:0.000}}{{'\t'}}{{M23:0.000}}
            {{'\t'}}{{M30:0.000}}{{'\t'}}{{M31:0.000}}{{'\t'}}{{M32:0.000}}{{'\t'}}{{M33:0.000}}
            }
            """;

    public static Mat4x4 operator *(Mat4x4 a, Mat4x4 b)
    {
        var o = new Mat4x4();
        for (int c = 0; c < 4; c++)
            for (int r = 0; r < 4; r++)
                o[r, c] = a[r, 0] * b[0, c] + a[r, 1] * b[1, c] + a[r, 2] * b[2, c] + a[r, 3] * b[3, c];
        return o;
    }

    public static Mat4x4 RotateX(float angle)
        => new Mat4x4
        {
            M00 = 1f,
            M11 = MathF.Cos(angle),
            M12 = MathF.Sin(angle),
            M21 = -MathF.Sin(angle),
            M22 = MathF.Cos(angle),
            M33 = 1f
        };

    public static Mat4x4 RotateY(float angle)
        => new Mat4x4
        {
            M00 = MathF.Cos(angle),
            M02 = MathF.Sin(angle),
            M20 = -MathF.Sin(angle),
            M11 = 1f,
            M22 = MathF.Cos(angle),
            M33 = 1f
        };

    public static Mat4x4 RotateZ(float angle)
        => new Mat4x4
        {
            M00 = MathF.Cos(angle),
            M01 = MathF.Sin(angle),
            M10 = -MathF.Sin(angle),
            M11 = MathF.Cos(angle),
            M22 = 1f,
            M33 = 1f
        };

    public static Mat4x4 Translate(Vec4d vec)
        => Translate(vec.X, vec.Y, vec.Z);
    public static Mat4x4 Translate(float x, float y, float z)
        => new Mat4x4
        {
            M00 = 1f,
            M11 = 1f,
            M22 = 1f,
            M33 = 1f,
            M30 = x,
            M31 = y,
            M32 = z
        };

    public static Mat4x4 Scale(float scale)
        => Scale(scale, scale, scale);
    public static Mat4x4 Scale(float x, float y, float z)
        => new Mat4x4
        {
            M00 = x,
            M11 = y,
            M22 = z,
            M33 = 1f
        };

    public static Mat4x4 Frustum(float fov, float aspectRatio, float near, float far)
        => new Mat4x4
        {
            M00 = aspectRatio * fov,
            M11 = fov,
            M22 = far / (far - near),
            M32 = (-far * near) / (far - near),
            M23 = 1f,
            M33 = 0f
        };

    public static Mat4x4 Orthographic(float left, float right, float bottom, float top, float near, float far)
        => new Mat4x4
        {
            M00 = 2f / (right - left),
            M11 = 2f / (top - bottom),
            M22 = -2f / (far - near),
            M30 = -(right + left) / (right - left),
            M31 = -(top + bottom) / (top - bottom),
            M32 = -(far + near) / (far - near),
            M33 = 1f
        };

    public static Mat4x4 PointAt(Vec4d pos, Vec4d target, Vec4d up)
    {
        var forward = (target - pos).Normalize();
        var newUp = (up - forward * up.Dot(forward)).Normalize();
        var right = newUp.Cross(forward);

        return new Mat4x4
        {
            #pragma warning disable format

            M00 = right.X,      M01 = right.Y,      M02 = right.Z,      M03 = 0f,
            M10 = newUp.X,      M11 = newUp.Y,      M12 = newUp.Z,      M13 = 0f,
            M20 = forward.X,    M21 = forward.Y,    M22 = forward.Z,    M23 = 0f,
            M30 = pos.X,        M31 = pos.Y,        M32 = pos.Z,        M33 = 1f

            #pragma warning restore format
        };
    }
}
