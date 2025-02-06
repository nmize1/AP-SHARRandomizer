using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System.Drawing;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVTrafficBodyDrawable@@")]
public class TrafficBodyDrawable : tDrawable
{
    public TrafficBodyDrawable(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint FadeAlphaOffset = NameOffset + sizeof(long);
    public int FadeAlpha
    {
        get => ReadInt32(FadeAlphaOffset);
        set => WriteInt32(FadeAlphaOffset, value);
    }

    internal const uint FadingOffset = FadeAlphaOffset + sizeof(int);
    public bool Fading
    {
        get => ReadBoolean(FadingOffset);
        set => WriteBoolean(FadingOffset, value);
    }

    internal const uint BodyPropDrawableOffset = FadingOffset + 4; // Padding
    public tDrawable BodyPropDrawable => Memory.ClassFactory.Create<tDrawable>(ReadUInt32(BodyPropDrawableOffset));

    internal const uint BodyShaderOffset = BodyPropDrawableOffset + sizeof(uint);
    public tShader BodyShader => Memory.ClassFactory.Create<tShader>(ReadUInt32(BodyShaderOffset));

    internal const uint DesiredColourOffset = BodyShaderOffset + sizeof(uint);
    public Color DesiredColour
    {
        get => ReadStruct<Color>(DesiredColourOffset);
        set => WriteStruct(DesiredColourOffset, value);
    }
}
