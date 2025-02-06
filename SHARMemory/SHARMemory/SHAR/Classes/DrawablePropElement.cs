using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVDrawablePropElement@tCompositeDrawable@@")]
public class DrawablePropElement : DrawableElement
{
    public DrawablePropElement(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public tDrawable Drawable => Memory.ClassFactory.Create<tDrawable>(ReadUInt32(20));
}
