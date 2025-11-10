using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes.Scrooby;
[ClassFactory.TypeInfoName(".?AVBoundedDrawable@Scrooby@@")]
public class BoundedDrawable : HasBoundingBox
{
    public BoundedDrawable(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ScroobyBoundedDrawableVFTableOffset = 0;

    internal const uint ScroobyBoundedDrawableVBTableOffset = ScroobyBoundedDrawableVFTableOffset + sizeof(uint);

    internal new const uint ScroobyDrawableVFTableOffset = ScroobyBoundedDrawableVBTableOffset + sizeof(uint);

    internal new const uint ScroobyHasBoundingBoxVFTableOffset = ScroobyDrawableVFTableOffset + sizeof(uint);

    internal new const uint ScroobyHasBoundingBoxVBTableOffset = ScroobyHasBoundingBoxVFTableOffset + sizeof(uint);
}
