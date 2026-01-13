using SHARMemory.Memory;
using System;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(ShortRectExtentsStruct))]
public struct ShortRectExtents
{
    public const int Size = sizeof(short) * 4;

    public short XMin;
    public short XMax;
    public short YMin;
    public short YMax;

    public ShortRectExtents(short xMin, short xMax, short yMin, short yMax)
    {
        XMin = xMin;
        XMax = xMax;
        YMin = yMin;
        YMax = yMax;
    }

    public override readonly string ToString() => $"<{XMin}, {XMax}, {YMin}, {YMax}>";
}

internal class ShortRectExtentsStruct : Struct
{
    public override int Size => ShortRectExtents.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        short XMin = BitConverter.ToInt16(Bytes, Offset);
        Offset += sizeof(short);
        short XMax = BitConverter.ToInt16(Bytes, Offset);
        Offset += sizeof(short);
        short YMin = BitConverter.ToInt16(Bytes, Offset);
        Offset += sizeof(short);
        short YMax = BitConverter.ToInt16(Bytes, Offset);
        return new ShortRectExtents(XMin, XMax, YMin, YMax);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not ShortRectExtents Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(ShortRectExtents)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.XMin).CopyTo(Buffer, Offset);
        Offset += sizeof(short);
        BitConverter.GetBytes(Value2.XMax).CopyTo(Buffer, Offset);
        Offset += sizeof(short);
        BitConverter.GetBytes(Value2.YMin).CopyTo(Buffer, Offset);
        Offset += sizeof(short);
        BitConverter.GetBytes(Value2.YMax).CopyTo(Buffer, Offset);
    }
}
