using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiSystem@@")]
public class CGuiSystem : CGuiEntity
{
    public CGuiSystem(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ScroobyLoadProjectCallbackVFTableOffset = ParentOffset + sizeof(uint);

    internal const uint GameDataHandlerVFTableOffset = ScroobyLoadProjectCallbackVFTableOffset + sizeof(uint);

    //internal const uint 
}
