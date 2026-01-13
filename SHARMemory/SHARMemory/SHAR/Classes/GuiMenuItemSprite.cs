using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AUGuiMenuItemSprite@@")]
public class GuiMenuItemSprite : GuiMenuItem
{
    public GuiMenuItemSprite(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }
}
