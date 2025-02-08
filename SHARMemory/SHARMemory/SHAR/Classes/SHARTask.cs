using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVTask@@")]
public class SHARTask : tRefCounted
{
    public enum Statuses
    {
        Done,
        Failed,
        Running,
        Sleeping
    }

    public SHARTask(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint StatusOffset = RefCountOffset + sizeof(uint);
    public Statuses Status
    {
        get => (Statuses)ReadUInt32(StatusOffset);
        set => WriteUInt32(StatusOffset, (uint)value);
    }
}
