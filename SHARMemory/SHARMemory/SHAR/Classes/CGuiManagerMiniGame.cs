using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiManagerMiniGame@@")]
public class CGuiManagerMiniGame : CGuiManager
{
    public CGuiManagerMiniGame(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }
}
