using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVBehaviour@@")]
public class Behaviour : tRefCounted
{
    public Behaviour(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint IsMutuallyExclusiveOffset = RefCountOffset + sizeof(uint);
    public bool IsMutuallyExclusive
    {
        get => ReadBoolean(IsMutuallyExclusiveOffset);
        set => WriteBoolean(IsMutuallyExclusiveOffset, value);
    }
}
