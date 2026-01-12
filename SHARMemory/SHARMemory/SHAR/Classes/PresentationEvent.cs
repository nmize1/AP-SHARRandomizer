using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVPresentationEvent@@")]
public class PresentationEvent : Class
{
    public PresentationEvent(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public string FileName => ReadString(8, System.Text.Encoding.UTF8, 64);

    public bool AutoPlay
    {
        get => ReadBitfield(73, 0);
        set => WriteBitfield(73, 0, value);
    }

    public bool ClearWhenDone
    {
        get => ReadBitfield(73, 1);
        set => WriteBitfield(73, 1, value);
    }

    public bool Loaded
    {
        get => ReadBitfield(73, 2);
        set => WriteBitfield(73, 2, value);
    }

    public bool KeepLayersFrozen
    {
        get => ReadBitfield(73, 3);
        set => WriteBitfield(73, 3, value);
    }

    public bool IsSkippable
    {
        get => ReadBitfield(73, 4);
        set => WriteBitfield(73, 4, value);
    }
}
