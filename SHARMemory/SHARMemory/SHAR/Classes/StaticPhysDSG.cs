using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVStaticPhysDSG@@")]
public class StaticPhysDSG : CollisionEntityDSG
{
    public StaticPhysDSG(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint BBoxOffset = RenderLayerOffset + sizeof(uint);
    public Box3D BBox
    {
        get => ReadStruct<Box3D>(BBoxOffset);
        set => WriteStruct(BBoxOffset, value);
    }

    internal const uint SphereOffset = BBoxOffset + Box3D.Size;
    public Sphere Sphere
    {
        get => ReadStruct<Sphere>(SphereOffset);
        set => WriteStruct(SphereOffset, value);
    }

    internal const uint PositionOffset = SphereOffset + Sphere.Size;
    public virtual Vector3 Position
    {
        get => ReadStruct<Vector3>(PositionOffset);
        set => WriteStruct(PositionOffset, value);
    }

    internal const uint SimStateOffset = PositionOffset + Vector3.Size;
    public SimState SimState => Memory.ClassFactory.Create<SimState>(ReadUInt32(SimStateOffset));

    internal const uint ShadowOffset = SimStateOffset + sizeof(uint);
    public tDrawable Shadow => Memory.ClassFactory.Create<tDrawable>(ReadUInt32(ShadowOffset));

    internal const uint ShadowMatrixOffset = ShadowOffset + sizeof(uint);
    public Matrix4x4? ShadowMatrix
    {
        get
        {
            var address = ReadUInt32(ShadowMatrixOffset);
            if (address == 0)
                return null;

            return Memory.ReadStruct<Matrix4x4>(address);
        }
    }
}
