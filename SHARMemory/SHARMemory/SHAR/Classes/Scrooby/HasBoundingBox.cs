using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes.Scrooby;
[ClassFactory.TypeInfoName(".?AVHasBoundingBox@Scrooby@@")]
public class HasBoundingBox : Drawable
{
    public HasBoundingBox(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ScroobyHasBoundingBoxVFTableOffset = 0;

    internal const uint ScroobyHasBoundingBoxVBTableOffset = ScroobyHasBoundingBoxVFTableOffset + sizeof(uint);

    internal new const uint ScroobyDrawableVFTableOffset = ScroobyHasBoundingBoxVBTableOffset + sizeof(uint);
}
