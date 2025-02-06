using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVtPolySkin@@")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical naming")]
public class tPolySkin : tDrawablePose
{
    public tPolySkin(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public PointerArray<tPrimGroup> PrimGroups => PointerArrayExtensions.FromPtrArray<tPrimGroup>(Memory, this, 32);
}
