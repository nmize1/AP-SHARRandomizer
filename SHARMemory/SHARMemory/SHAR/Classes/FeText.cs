using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System.Drawing;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVFeText@@")]
public class FeText : FeBoundedDrawable
{
    public enum TextModes
    {
        Overlap,
        Clip,
        Wrap
    }

    public FeText(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint FeParentVFTableOffset = OffsetYOffset + sizeof(float);
    //internal const uint FeParentVBTableOffset = FeParentVFTableOffset + sizeof(uint);

    internal const uint ChildIterOffset = FeParentVFTableOffset + sizeof(uint);
    // tLinearTable::RawIterator*

    internal const uint CurChildIndexOffset = ChildIterOffset + sizeof(uint);
    public int CurChildIndex
    {
        get => ReadInt32(CurChildIndexOffset);
        set => WriteInt32(CurChildIndexOffset, value);
    }

    internal const uint ChildrenOffset = CurChildIndexOffset + sizeof(int);
    // tLinearTable*

    internal const uint FeTextVBTableOffset = ChildrenOffset + sizeof(uint);

    internal const uint IndexOffset = FeTextVBTableOffset + sizeof(uint);
    public int Index
    {
        get => ReadInt32(IndexOffset);
        set => WriteInt32(IndexOffset, value);
    }

    internal const uint TextStyleOffset = IndexOffset + sizeof(int);
    public uint TextStyle
    {
        get => ReadUInt32(TextStyleOffset);
        set => WriteUInt32(TextStyleOffset, value);
    }

    internal const uint XShadowOffsetOffset = TextStyleOffset + sizeof(uint);
    public float XShadowOffset
    {
        get => ReadSingle(XShadowOffsetOffset);
        set => WriteSingle(XShadowOffsetOffset, value);
    }

    internal const uint YShadowOffsetOffset = XShadowOffsetOffset + sizeof(float);
    public float YShadowOffset
    {
        get => ReadSingle(YShadowOffsetOffset);
        set => WriteSingle(YShadowOffsetOffset, value);
    }

    internal const uint OriginalColourOffset = YShadowOffsetOffset + sizeof(float);
    public Color OriginalColour
    {
        get => ReadStruct<Color>(OriginalColourOffset);
        set => WriteStruct(OriginalColourOffset, value);
    }

    internal const uint ShadowColourOffset = OriginalColourOffset + sizeof(uint);
    public Color ShadowColour
    {
        get => ReadStruct<Color>(ShadowColourOffset);
        set => WriteStruct(ShadowColourOffset, value);
    }

    internal const uint FontOffset = ShadowColourOffset + sizeof(uint);
    //public tFont Font => Memory.ClassFactory.Create<tFont>(ReadUInt32(FontOffset));

    internal const uint TextModeOffset = FontOffset + sizeof(uint);
    public TextModes TextMode
    {
        get => (TextModes)ReadInt32(TextModeOffset);
        set => WriteInt32(TextModeOffset, (int)value);
    }

    internal const uint BufferOffset = TextModeOffset + sizeof(int);
    public string Buffer
    {
        get => ReadNullStringPointer(BufferOffset, System.Text.Encoding.Unicode);
        //set => WriteNullStringPointer(BufferOffset, value, System.Text.Encoding.Unicode);
    }

    internal const uint OverrideStringBufferOffset = BufferOffset + sizeof(uint);
    public bool OverrideStringBuffer
    {
        get => ReadBitfield(OverrideStringBufferOffset, 0);
        set => WriteBitfield(OverrideStringBufferOffset, 0, value);
    }

    internal const uint DisplayShadowOffset = OverrideStringBufferOffset + 0;
    public bool DisplayShadow
    {
        get => ReadBitfield(DisplayShadowOffset, 1);
        set => WriteBitfield(DisplayShadowOffset, 1, value);
    }

    internal const uint DisplayOutlineOffset = DisplayShadowOffset + 0;
    public bool DisplayOutline
    {
        get => ReadBitfield(DisplayOutlineOffset, 2);
        set => WriteBitfield(DisplayOutlineOffset, 2, value);
    }

    internal const uint IsBoundingBoxStretchedOffset = DisplayOutlineOffset + 0;
    public bool IsBoundingBoxStretched
    {
        get => ReadBitfield(IsBoundingBoxStretchedOffset, 3);
        set => WriteBitfield(IsBoundingBoxStretchedOffset, 3, value);
    }

    internal const uint OutlineColourOffset = IsBoundingBoxStretchedOffset + 4; // Padding
    public Color OutlineColour
    {
        get => ReadStruct<Color>(OutlineColourOffset);
        set => WriteStruct(OutlineColourOffset, value);
    }

    internal const uint RectExtentsOffset = OutlineColourOffset + sizeof(uint);
    public ShortRectExtents RectExtents
    {
        get => ReadStruct<ShortRectExtents>(RectExtentsOffset);
        set => WriteStruct(RectExtentsOffset, value);
    }
}
