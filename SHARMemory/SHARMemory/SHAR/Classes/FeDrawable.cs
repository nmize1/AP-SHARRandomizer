using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVFeDrawable@@")]
public class FeDrawable : FeEntity
{
    public FeDrawable(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint FeDrawableVBTableOffset = NameOffset + sizeof(long);

    internal const uint ParentOffset = FeDrawableVBTableOffset + sizeof(uint);
    public FeDrawable Parent => Memory.ClassFactory.Create<FeDrawable>(ReadUInt32(ParentOffset));

    internal const uint VisibleOffset = ParentOffset + sizeof(uint);
    public bool Visible
    {
        get => ReadBoolean(VisibleOffset);
        set => WriteBoolean(VisibleOffset, value);
    }
}
