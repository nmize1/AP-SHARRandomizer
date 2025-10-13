using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVIntersectionList@@")]
public class IntersectionList : Class
{
    public IntersectionList(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint IntersectionListVFTableOffset = 0;

    internal const uint StaticCollisionListOffset = IntersectionListVFTableOffset + sizeof(uint);

    internal const uint DynamicCollisionListOffset = StaticCollisionListOffset + 12;

    internal const uint AnimPhysCollisionListOffset = DynamicCollisionListOffset + 16;

    internal const uint FenceListOffset = AnimPhysCollisionListOffset + 12;

    internal const uint Size = FenceListOffset + 12;

    // TODO
}
