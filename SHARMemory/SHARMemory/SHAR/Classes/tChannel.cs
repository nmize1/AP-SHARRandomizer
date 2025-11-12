using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVtChannel@@")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical naming")]
public class tChannel : tNonCopyable
{
    public enum DataTypes : ushort
    {
        Unassigned,
        Int,
        Float1,
        Float2,
        Vector,
        Rotation,
        String,
        Entity,
        Bool,
        Colour,
        Event
    }

    public tChannel(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint tChannelVFTableOffset = 0;

    internal const uint MemoryBlockOffset = tChannelVFTableOffset + sizeof(uint);
    public tAnimationMemoryBlock MemoryBlock => Memory.ClassFactory.Create<tAnimationMemoryBlock>(ReadUInt32(MemoryBlockOffset));

    internal const uint ChannelCodeOffset = MemoryBlockOffset + sizeof(uint);
    public uint ChannelCode
    {
        get => ReadUInt32(ChannelCodeOffset);
        set => WriteUInt32(ChannelCodeOffset, value);
    }

    internal const uint InterpolateAndDataTypeOffset = ChannelCodeOffset + sizeof(uint);
    private byte InterpolateAndDataType
    {
        get => ReadByte(InterpolateAndDataTypeOffset);
        set => WriteByte(InterpolateAndDataType, value);
    }
    public bool Interpolate
    {
        get => (InterpolateAndDataType & 0b00000001) != 0;
        set
        {
            if (value)
                InterpolateAndDataType |= 0b00000001;
            else
                InterpolateAndDataType &= 0b11111110;
        }
    }
    public DataTypes DataType
    {
        get => (DataTypes)(InterpolateAndDataType >> 1);
        set
        {
            var currValue = InterpolateAndDataType & 0b00000001;
            currValue |= ((ushort)value << 1) & 0b11111110;
            InterpolateAndDataType = (byte)currValue;
        }
    }

    internal const uint NumKeysOffset = InterpolateAndDataTypeOffset + sizeof(ushort);
    public short NumKeys
    {
        get => ReadInt16(NumKeysOffset);
        set => WriteInt16(NumKeysOffset, value);
    }

    internal const uint FramesOffset = NumKeysOffset + sizeof(short);
    public StructArray<short> Frames => new(Memory, ReadUInt32(FramesOffset), sizeof(short), NumKeys);
}
