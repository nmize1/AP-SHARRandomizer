using SHARMemory.Memory;
using SHARMemory.SHAR.Classes;
using System;
using System.Drawing;

namespace SHARMemory.SHAR.Structs;

[Struct(typeof(MoodLightingStruct))]
public struct MoodLighting
{
    public const int Size = sizeof(uint) + sizeof(int) + sizeof(int) + sizeof(uint) + sizeof(float) + sizeof(int);

    public tLightGroup SunGroup;

    public Color SrcModulus;

    public Color DstModulus;

    public StructArray<Color> Originals;

    public float Transition;

    public int VolumeCount;

    public MoodLighting(tLightGroup sunGroup, Color srcModulus, Color dstModulus, StructArray<Color> originals, float transition, int volumeCount)
    {
        SunGroup = sunGroup;
        SrcModulus = srcModulus;
        DstModulus = dstModulus;
        Originals = originals;
        Transition = transition;
        VolumeCount = volumeCount;
    }

    public override readonly string ToString() => $"{SunGroup} | {SrcModulus} | {DstModulus} | {Originals} | {Transition} | {VolumeCount}";
}

internal class MoodLightingStruct : Struct
{
    public override int Size => MoodLighting.Size;

    public override object FromBytes(ProcessMemory Memory, byte[] Bytes, int Offset = 0)
    {
        tLightGroup SunGroup = Memory.ClassFactory.Create<tLightGroup>(BitConverter.ToUInt32(Bytes, Offset));
        Offset += sizeof(uint);
        Color SrcModulus = Memory.StructFromBytes<Color>(Bytes, Offset);
        Offset += sizeof(int);
        Color DstModulus = Memory.StructFromBytes<Color>(Bytes, Offset);
        Offset += sizeof(int);
        StructArray<Color> Originals = new(Memory, BitConverter.ToUInt32(Bytes, Offset), sizeof(int), SunGroup?.NumLights ?? 0);
        Offset += sizeof(uint);
        float Transition = BitConverter.ToSingle(Bytes, Offset);
        Offset += sizeof(float);
        int VolumeCount = BitConverter.ToInt32(Bytes, Offset);
        return new MoodLighting(SunGroup, SrcModulus, DstModulus, Originals, Transition, VolumeCount);
    }

    public override void ToBytes(ProcessMemory Memory, object Value, byte[] Buffer, int Offset = 0)
    {
        if (Value is not MoodLighting Value2)
            throw new ArgumentException($"Argument '{nameof(Value)}' must be of type '{nameof(MoodLighting)}'.", nameof(Value));

        BitConverter.GetBytes(Value2.SunGroup?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        Memory.BytesFromStruct(Value2.SrcModulus, Buffer, Offset);
        Offset += sizeof(int);
        Memory.BytesFromStruct(Value2.DstModulus, Buffer, Offset);
        Offset += sizeof(int);
        BitConverter.GetBytes(Value2.Originals?.Address ?? 0).CopyTo(Buffer, Offset);
        Offset += sizeof(uint);
        BitConverter.GetBytes(Value2.Transition).CopyTo(Buffer, Offset);
        Offset += sizeof(float);
        BitConverter.GetBytes(Value2.VolumeCount).CopyTo(Buffer, Offset);
    }
}
