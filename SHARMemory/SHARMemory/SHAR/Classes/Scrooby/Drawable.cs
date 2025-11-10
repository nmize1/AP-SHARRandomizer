using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes.Scrooby;
[ClassFactory.TypeInfoName(".?AVDrawable@Scrooby@@")]
public class Drawable : Class
{
    public Drawable(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ScroobyDrawableVFTableOffset = 0;
}
