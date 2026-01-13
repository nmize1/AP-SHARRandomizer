using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AUGuiMenuItemText@@")]
public class GuiMenuItemText : GuiMenuItem
{
    public GuiMenuItemText(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ItemOffset = SliderOffset + 36; // TODO: ImageSlider.Size
    public FeText Item => Memory.ClassFactory.Create<FeText>(ReadUInt32(ItemOffset));

    internal const uint ItemValueOffset = ItemOffset + sizeof(uint);
    public FeText ItemValue => Memory.ClassFactory.Create<FeText>(ReadUInt32(ItemValueOffset));
}
