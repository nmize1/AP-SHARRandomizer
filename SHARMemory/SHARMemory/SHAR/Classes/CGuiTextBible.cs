using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiTextBible@@")]
public class CGuiTextBible : Class
{
    public CGuiTextBible(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint CGuiTextBibleVFTableOffset = 0;
}
