using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVStaticEntityDSG@@")]
public class StaticEntityDSG : IEntityDSG
{
    public enum Geo
    {
        NotGeo,
        Geo,
        IsShadow
    }

    public StaticEntityDSG(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint PosnOffset = SpatialNodeOffset + sizeof(uint);
    public Vector3 Posn
    {
        get => ReadStruct<Vector3>(PosnOffset);
        set => WriteStruct(PosnOffset, value);
    }

    internal const uint IsGeoOffset = PosnOffset + Vector3.Size;
    public Geo IsGeo
    {
        get => (Geo)ReadInt32(IsGeoOffset);
        set => WriteInt32(IsGeoOffset, (int)value);
    }

    internal const uint DrawstuffOffset = IsGeoOffset + sizeof(int);
    public tDrawable Drawstuff => Memory.ClassFactory.Create<tDrawable>(ReadUInt32(DrawstuffOffset));
}
