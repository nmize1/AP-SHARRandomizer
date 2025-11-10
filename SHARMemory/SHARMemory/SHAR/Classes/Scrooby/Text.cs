using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes.Scrooby;
[ClassFactory.TypeInfoName(".?AVText@Scrooby@@")]
public class Text : BoundedDrawable
{
    public Text(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ScroobyTextVFTableOffset = 0;

    internal const uint ScroobyTextVBTableOffset = ScroobyTextVFTableOffset + sizeof(uint);

    internal new const uint ScroobyDrawableVFTableOffset = ScroobyTextVBTableOffset + sizeof(uint);

    internal new const uint ScroobyHasBoundingBoxVFTableOffset = ScroobyDrawableVFTableOffset + sizeof(uint);

    internal new const uint ScroobyHasBoundingBoxVBTableOffset = ScroobyHasBoundingBoxVFTableOffset + sizeof(uint);

    internal new const uint ScroobyBoundedDrawableVFTableOffset = ScroobyHasBoundingBoxVBTableOffset + sizeof(uint);

    internal new const uint ScroobyBoundedDrawableVBTableOffset = ScroobyBoundedDrawableVFTableOffset + sizeof(uint);
}
