using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(MissionDisplayInfoStruct))]
public struct MissionDisplayInfo
{
    public const int Size = sizeof(uint) + sizeof(uint) + sizeof(uint);

    public FeEntity Number;

    public FeEntity Title;

    public FeEntity Status;

    public MissionDisplayInfo(FeEntity number, FeEntity title, FeEntity status)
    {
        Number = number;
        Title = title;
        Status = status;
    }

    public override readonly string ToString() => $"{Number} | {Title} | {Status}";
}

internal class MissionDisplayInfoStruct : Struct
{
    public override int Size => MissionDisplayInfo.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        FeEntity Number = Memory.ClassFactory.Create<FeEntity>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        FeEntity Title = Memory.ClassFactory.Create<FeEntity>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        FeEntity Status = Memory.ClassFactory.Create<FeEntity>(BitConverter.ToUInt32(Bytes, Offset));
        return new MissionDisplayInfo(Number, Title, Status);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not MissionDisplayInfo Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(MissionDisplayInfo)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.Number?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.Title?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.Status?.Address ?? 0).CopyTo(Buffer, Offset);
    }
}
