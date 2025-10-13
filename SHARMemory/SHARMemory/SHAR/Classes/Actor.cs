using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVActor@@")]
public class Actor : tRefCounted
{
    public Actor(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint StatePropOffset = RefCountOffset + sizeof(uint);
    public ActorDSG StateProp => Memory.ClassFactory.Create<ActorDSG>(ReadUInt32(StatePropOffset));

    internal const uint ShouldDespawnOffset = StatePropOffset + sizeof(uint);
    public bool ShouldDespawn
    {
        get => ReadBoolean(ShouldDespawnOffset);
        set => WriteBoolean(ShouldDespawnOffset, value);
    }

    internal const uint IntersectionListOffset = ShouldDespawnOffset + 4; // Padding
    public IntersectionList IntersectionList => Memory.ClassFactory.Create<IntersectionList>(Address + IntersectionListOffset);

    internal const uint IntersectionSphereOffset = IntersectionListOffset + IntersectionList.Size;
    public Sphere IntersectionSphere
    {
        get => ReadStruct<Sphere>(IntersectionSphereOffset);
        set => WriteStruct(IntersectionSphereOffset, value);
    }

    internal const uint PreviousPositionOffset = IntersectionSphereOffset + Sphere.Size;
    public Vector3 PreviousPosition
    {
        get => ReadStruct<Vector3>(PreviousPositionOffset);
        set => WriteStruct(PreviousPositionOffset, value);
    }

    internal const uint IsInDSGOffset = PreviousPositionOffset + Vector3.Size;
    public bool IsInDSG
    {
        get => ReadBoolean(IsInDSGOffset);
        set => WriteBoolean(IsInDSGOffset, value);
    }
}
