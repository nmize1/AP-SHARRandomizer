using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVLocator@@")]
public class Locator : tEntity
{
    [Flags]
    public enum LocatorFlags : uint
    {
        None = 0,
        Active = 1 << 0,
        Drawn = 1 << 1,
        Used = 1 << 2,
        Interior = 1 << 3,
    }

    public Locator(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint IHudMapIconLocatorVFTableOffset = NameOffset + sizeof(ulong);

    internal const uint IDOffset = IHudMapIconLocatorVFTableOffset + sizeof(uint);
    public uint ID
    {
        get => ReadUInt32(IDOffset);
        set => WriteUInt32(IDOffset, value);
    }

    internal const uint DataOffset = IDOffset + sizeof(uint);
    public uint Data
    {
        get => ReadUInt32(DataOffset);
        set => WriteUInt32(DataOffset, value);
    }

    internal const uint FlagsOffset = DataOffset + sizeof(uint);
    public LocatorFlags Flags
    {
        get => (LocatorFlags)ReadUInt32(FlagsOffset);
        set => WriteUInt32(FlagsOffset, (uint)value);
    }

    internal const uint LocationOffset = FlagsOffset + sizeof(uint);
    public Vector3 Location
    {
        get => ReadStruct<Vector3>(LocationOffset);
        set => WriteStruct(LocationOffset, value);
    }
}
