using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVPresentationEvent@@")]
public class PresentationEvent : Class
{
    public PresentationEvent(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public bool AutoPlay
    {
        get => ReadBitfield(4, 0);
        set => WriteBitfield(4, 0, value);
    }

    public bool ClearWhenDone
    {
        get => ReadBitfield(4, 1);
        set => WriteBitfield(4, 1, value);
    }

    public bool Loaded
    {
        get => ReadBitfield(4, 2);
        set => WriteBitfield(4, 2, value);
    }

    public bool KeepLayersFrozen
    {
        get => ReadBitfield(4, 3);
        set => WriteBitfield(4, 3, value);
    }

    public bool IsSkippable
    {
        get => ReadBitfield(4, 4);
        set => WriteBitfield(4, 4, value);
    }
}
