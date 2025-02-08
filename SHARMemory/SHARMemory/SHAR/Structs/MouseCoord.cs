using SHARMemory.Memory;
using System;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(MouseCoordStruct))]
public struct MouseCoord
{
    public const int Size = sizeof(float) * 2;

    public float X;
    public float Y;

    public MouseCoord(float value)
    {
        X = value;
        Y = value;
    }

    public MouseCoord(float x, float y)
    {
        X = x;
        Y = y;
    }

    public override readonly string ToString() => $"<{X:0.00000}, {Y:0.00000}>";
}

internal class MouseCoordStruct : Struct
{
    public override int Size => MouseCoord.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        float X = BitConverter.ToSingle(Bytes, Offset);
        Offset += sizeof(float);
        float Y = BitConverter.ToSingle(Bytes, Offset);
        return new MouseCoord(X, Y);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not MouseCoord Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(MouseCoord)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.X).CopyTo(Buffer, Offset);
        Offset += sizeof(float);
        BitConverter.GetBytes(Value2.Y).CopyTo(Buffer, Offset);
    }
}
