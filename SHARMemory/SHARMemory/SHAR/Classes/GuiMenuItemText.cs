using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AUGuiMenuItemText@@")]
public class GuiMenuItemText : GuiMenuItem
{
    public GuiMenuItemText(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ItemOffset = SliderOffset + 36; // TODO: ImageSlider.Size
    public FeDrawable Item => Memory.ClassFactory.Create<FeDrawable>(ReadUInt32(ItemOffset)); // TODO: FeText

    internal const uint ItemValueOffset = ItemOffset + sizeof(uint);
    public FeDrawable ItemValue => Memory.ClassFactory.Create<FeDrawable>(ReadUInt32(ItemValueOffset)); // TODO: FeText
}
