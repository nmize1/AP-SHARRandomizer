using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVtLightGroup@@")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical naming")]
public class tLightGroup : Class
{
    public tLightGroup(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public int NumLights
    {
        get => ReadInt32(16);
        set => WriteInt32(16, value);
    }

    public int CurNumLights
    {
        get => ReadInt32(20);
        set => WriteInt32(20, value);
    }

    public PointerArray<tLight> Lights => PointerArrayExtensions.FromPtrArray<tLight>(Memory, this, 24);
}
