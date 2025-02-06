using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System.Drawing;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVtLight@@")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical naming")]
public class tLight : Class
{
    public enum IlluminationTypes
    {
        Positive,
        Zero,
        Negative,
    }

    public tLight(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    private const uint ColourOffset = 16;
    public Color Colour
    {
        get => ReadStruct<Color>(ColourOffset);
        set => WriteStruct(16, value);
    }

    private const uint PositionOffset = ColourOffset + sizeof(int);
    public Vector3 Position
    {
        get => ReadStruct<Vector3>(PositionOffset);
        set => WriteStruct(PositionOffset, value);
    }

    private const uint SlotOffset = PositionOffset + Vector3.Size;
    public uint Slot
    {
        get => ReadUInt32(SlotOffset);
        set => WriteUInt32(SlotOffset, value);
    }

    public bool Active
    {
        get => ReadBoolean(36);
        set => WriteBoolean(36, value);
    }

    public bool Enabled
    {
        get => ReadBoolean(37);
        set => WriteBoolean(37, value);
    }

    public bool IsShadowCaster
    {
        get => ReadBoolean(38);
        set => WriteBoolean(38, value);
    }

    public bool Animated
    {
        get => ReadBoolean(39);
        set => WriteBoolean(39, value);
    }

    public DecayRange DecayRange
    {
        get => ReadStruct<DecayRange>(40);
        set => WriteStruct(40, value);
    }

    public IlluminationTypes IlluminationType
    {
        get => (IlluminationTypes)ReadInt32(76);
        set => WriteInt32(76, (int)value);
    }
}
