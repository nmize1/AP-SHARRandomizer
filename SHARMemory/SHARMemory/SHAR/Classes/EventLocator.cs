using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using static SHARMemory.SHAR.Globals;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVEventLocator@@")]
public class EventLocator : TriggerLocator
{
    public EventLocator(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint EventTypeOffset = PlayerIDOffset + sizeof(int);
    public LocatorEvents EventType
    {
        get => (LocatorEvents)ReadUInt32(EventTypeOffset);
        set => WriteUInt32(EventTypeOffset, (uint)value);
    }

    internal const uint MatrixOffset = EventTypeOffset + sizeof(uint);
    public Matrix4x4 Matrix
    {
        get => ReadStruct<Matrix4x4>(MatrixOffset);
        set => WriteStruct(MatrixOffset, value);
    }
}
