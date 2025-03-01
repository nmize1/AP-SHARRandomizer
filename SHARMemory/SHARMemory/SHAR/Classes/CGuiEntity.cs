using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiEntity@@")]
public class CGuiEntity : Class
{
    public CGuiEntity(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint CGuiEntityVFTableOffset = 0;

    internal const uint ParentOffset = CGuiEntityVFTableOffset + sizeof(uint);
    public CGuiEntity Parent => Memory.ClassFactory.Create<CGuiEntity>(ReadUInt32(ParentOffset));
}
