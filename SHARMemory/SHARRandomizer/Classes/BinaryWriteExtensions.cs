using System.Numerics;

namespace SHARRandomizer.Classes;

public static class BinaryWriterExtensions
{
    public static void Write(this BinaryWriter bw, Vector3 vec)
    {
        bw.Write(vec.X);
        bw.Write(vec.Y);
        bw.Write(vec.Z);
    }

    public static void Write(this BinaryWriter bw, Matrix4x4 mat)
    {
        bw.Write(mat.M11);
        bw.Write(mat.M12);
        bw.Write(mat.M13);
        bw.Write(mat.M14);
        bw.Write(mat.M21);
        bw.Write(mat.M22);
        bw.Write(mat.M23);
        bw.Write(mat.M24);
        bw.Write(mat.M31);
        bw.Write(mat.M32);
        bw.Write(mat.M33);
        bw.Write(mat.M34);
        bw.Write(mat.M41);
        bw.Write(mat.M42);
        bw.Write(mat.M43);
        bw.Write(mat.M44);
    }
}