using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System.Drawing;

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

    internal const uint MatrixOffset = VisibleOffset + 4; // Padding
    public Matrix4x4 Matrix
    {
        get => ReadStruct<Matrix4x4>(MatrixOffset);
        set => WriteStruct(MatrixOffset, value);
    }

    internal const uint ClipOffset = MatrixOffset + Matrix4x4.Size;
    public bool Clip
    {
        get => ReadBoolean(ClipOffset);
        set => WriteBoolean(ClipOffset, value);
    }

    internal const uint RectOffset = ClipOffset + 4; // Padding
    public pddiRect Rect
    {
        get => ReadStruct<pddiRect>(RectOffset);
        set => WriteStruct(RectOffset, value);
    }

    internal const uint LayerOffset = RectOffset + pddiRect.Size;
    public float Layer
    {
        get => ReadSingle(LayerOffset);
        set => WriteSingle(LayerOffset, value);
    }

    internal const uint ColourOffset = LayerOffset + sizeof(float);
    public Color Colour
    {
        get => ReadStruct<Color>(ColourOffset);
        set => WriteStruct(ColourOffset, value);
    }

    internal const uint AlphaOffset = ColourOffset + sizeof(uint);
    public float Alpha
    {
        get => ReadSingle(AlphaOffset);
        set => WriteSingle(AlphaOffset, value);
    }

    internal const uint PosXOffset = AlphaOffset + sizeof(float);
    public float PosX
    {
        get => ReadSingle(PosXOffset);
        set => WriteSingle(PosXOffset, value);
    }

    internal const uint PosYOffset = PosXOffset + sizeof(float);
    public float PosY
    {
        get => ReadSingle(PosYOffset);
        set => WriteSingle(PosYOffset, value);
    }
}
