using SHARMemory.Memory;
using System;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(pddiRectStruct))]
public struct pddiRect
{
    public const int Size = sizeof(int) * 4;

    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public pddiRect(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public override readonly string ToString() => $"<{Left}, {Top}, {Right}, {Bottom}>";
}

internal class pddiRectStruct : Struct
{
    public override int Size => pddiRect.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        int Left = BitConverter.ToInt32(Bytes, Offset);
        Offset += sizeof(int);
        int Top = BitConverter.ToInt32(Bytes, Offset);
        Offset += sizeof(int);
        int Right = BitConverter.ToInt32(Bytes, Offset);
        Offset += sizeof(int);
        int Bottom = BitConverter.ToInt32(Bytes, Offset);
        return new pddiRect(Left, Top, Right, Bottom);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not pddiRect Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(pddiRect)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.Left).CopyTo(Buffer, Offset);
        Offset += sizeof(int);
        BitConverter.GetBytes(Value2.Top).CopyTo(Buffer, Offset);
        Offset += sizeof(int);
        BitConverter.GetBytes(Value2.Right).CopyTo(Buffer, Offset);
        Offset += sizeof(int);
        BitConverter.GetBytes(Value2.Bottom).CopyTo(Buffer, Offset);
    }
}
