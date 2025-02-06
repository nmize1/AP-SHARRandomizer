using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVtCompositeDrawable@@")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical naming")]
public class tCompositeDrawable : tDrawablePose
{
    public tCompositeDrawable(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public Box3D BoundingBox
    {
        get => ReadStruct<Box3D>(28);
        set => WriteStruct(28, value);
    }

    public Sphere BoundingSphere
    {
        get => ReadStruct<Sphere>(52);
        set => WriteStruct(52, value);
    }

    public PointerArray<DrawableElement> Elements => PointerArrayExtensions.FromPtrArray<DrawableElement>(Memory, this, 68);

    public DisplayList DisplayList => Memory.ClassFactory.Create<DisplayList>(Address + 80);

    public bool Billboard
    {
        get => ReadBoolean(104);
        set => WriteBoolean(104, value);
    }
}
