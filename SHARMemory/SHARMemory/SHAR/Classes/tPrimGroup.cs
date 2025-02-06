using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVtPrimGroup@@")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical naming")]
public class tPrimGroup : Class
{
    public tPrimGroup(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public tShader Shader => Memory.ClassFactory.Create<tShader>(ReadUInt32(8));
}
