using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVInteriorManager@@")]
public class InteriorManager : Class
{
    public enum InteriorStates : uint
    {
        None,
        Enter,
        Exit,
        Inside
    }

    public InteriorManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint InteriorManagerVFTableOffset = 0;

    internal const uint InteriorStateOffset = InteriorManagerVFTableOffset + sizeof(uint);
    public InteriorStates InteriorState
    {
        get => (InteriorStates)ReadUInt32(InteriorStateOffset);
        set => WriteUInt32(InteriorStateOffset, (uint)value);
    }

    internal const uint EntryRequestedOffset = InteriorStateOffset + sizeof(uint);
    public bool EntryRequested
    {
        get => ReadBoolean(EntryRequestedOffset);
        set => WriteBoolean(EntryRequestedOffset, value);
    }

    //internal const uint InteriorLoadedOffset = 
}
