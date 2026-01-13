using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVFeBoundedDrawable@@")]
public class FeBoundedDrawable : FeDrawable
{
    // TODO: Find a better location for this
    public enum Justification
    {
        Left,
        Right,
        Top,
        Bottom,
        Center
    }

    public FeBoundedDrawable(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    //internal const uint FeBoundedDrawableVBTableOffset = PosYOffset + sizeof(float);

    internal const uint WidthOffset = PosYOffset + sizeof(uint);
    public int Width
    {
        get => ReadInt32(WidthOffset);
        set => WriteInt32(WidthOffset, value);
    }

    internal const uint HeightOffset = WidthOffset + sizeof(int);
    public int Height
    {
        get => ReadInt32(HeightOffset);
        set => WriteInt32(HeightOffset, value);
    }

    internal const uint HorizontalJustificationOffset = HeightOffset + sizeof(int);
    public Justification HorizontalJustification
    {
        get => (Justification)ReadInt32(HorizontalJustificationOffset);
        set => WriteInt32(HorizontalJustificationOffset, (int)value);
    }

    internal const uint VerticalJustificationOffset = HorizontalJustificationOffset + sizeof(int);
    public Justification VerticalJustification
    {
        get => (Justification)ReadInt32(VerticalJustificationOffset);
        set => WriteInt32(VerticalJustificationOffset, (int)value);
    }

    internal const uint DrawableOffset = VerticalJustificationOffset + sizeof(int);
    public tDrawable Drawable => Memory.ClassFactory.Create<tDrawable>(ReadUInt32(DrawableOffset));

    internal const uint IsDirtyOffset = DrawableOffset + sizeof(uint);
    public bool IsDirty
    {
        get => ReadBoolean(IsDirtyOffset);
        set => WriteBoolean(IsDirtyOffset, value);
    }

    internal const uint OffsetXOffset = IsDirtyOffset + 4; // Padding
    public float OffsetX
    {
        get => ReadSingle(OffsetXOffset);
        set => WriteSingle(OffsetXOffset, value);
    }

    internal const uint OffsetYOffset = OffsetXOffset + sizeof(float);
    public float OffsetY
    {
        get => ReadSingle(OffsetYOffset);
        set => WriteSingle(OffsetYOffset, value);
    }
}
