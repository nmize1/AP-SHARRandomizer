using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVDrawableElement@tCompositeDrawable@@")]
public class DrawableElement : Class
{
    public enum Types
    {
        Base,
        Prop,
        Skin,
        Effect
    }

    public DrawableElement(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint VFTableOffset = 0;

    internal const uint TypeOffset = VFTableOffset + sizeof(uint);
    public Types Type
    {
        get => (Types)ReadUInt32(TypeOffset);
        set => WriteUInt32(TypeOffset, (uint)value);
    }

    internal const uint VisibleOffset = TypeOffset + sizeof(uint);
    public bool Visible
    {
        get => ReadBoolean(VisibleOffset);
        set => WriteBoolean(VisibleOffset, value);
    }

    internal const uint LockVisibilityOffset = VisibleOffset + 1;
    public bool LockVisibility
    {
        get => ReadBoolean(LockVisibilityOffset);
        set => WriteBoolean(LockVisibilityOffset, value);
    }

    internal const uint IsTranslucentOffset = LockVisibilityOffset + 1;
    public bool IsTranslucent
    {
        get => ReadBoolean(IsTranslucentOffset);
        set => WriteBoolean(IsTranslucentOffset, value);
    }

    internal const uint SortOrderOffset = IsTranslucentOffset + 2; // Padding
    public float SortOrder
    {
        get => ReadSingle(SortOrderOffset);
        set => WriteSingle(SortOrderOffset, value);
    }

    internal const uint PoseOffset = SortOrderOffset + sizeof(float);
    // public tPose Pose => Memory.ClassFactory.Create<Pose>(ReadUInt32(PoseOffset));
}
