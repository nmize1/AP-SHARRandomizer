using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCollisionObject@sim@@")]
public class CollisionObject : Class
{
    public CollisionObject(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public SimState SimState => Memory.ClassFactory.Create<SimState>(ReadUInt32(24));

    public PhysicsProperties PhysicsProperties => Memory.ClassFactory.Create<PhysicsProperties>(ReadUInt32(28));

    public CollisionVolume CollisionVolume => Memory.ClassFactory.Create<CollisionVolume>(ReadUInt32(32));

    public int NumJoint
    {
        get => ReadInt32(36);
        set => WriteInt32(36, value);
    }

    public PointerArray<SelfCollision> SelfCollisionList => PointerArrayExtensions.FromPtrArray<SelfCollision>(Memory, this, 40);

    public CollisionVolumeOwner CollisionVolumeOwner => Memory.ClassFactory.Create<CollisionVolumeOwner>(Address + 60);

    public bool IsStatic
    {
        get => ReadBitfield(88, 0);
        set => WriteBitfield(88, 0, value);
    }

    public bool CollideWithStatic
    {
        get => ReadBitfield(88, 1);
        set => WriteBitfield(88, 1, value);
    }

    public int DefaultArea
    {
        get => ReadInt32(92);
        set => WriteInt32(92, value);
    }

    public bool HasMoved
    {
        get => ReadBitfield(96, 0);
        set => WriteBitfield(96, 0, value);
    }

    public bool HasRelocated
    {
        get => ReadBitfield(96, 1);
        set => WriteBitfield(96, 1, value);
    }

    public bool AutoPair
    {
        get => ReadBitfield(96, 2);
        set => WriteBitfield(96, 2, value);
    }

    public bool ManualUpdate
    {
        get => ReadBitfield(96, 3);
        set => WriteBitfield(96, 3, value);
    }

    public bool CollisionEnabled
    {
        get => ReadBitfield(96, 4);
        set => WriteBitfield(96, 4, value);
    }

    public bool SelfCollisionEnabled
    {
        get => ReadBitfield(96, 5);
        set => WriteBitfield(96, 5, value);
    }

    public bool RayCastingEnabled
    {
        get => ReadBitfield(96, 6);
        set => WriteBitfield(96, 6, value);
    }

    public int PossibleCollisionEvents
    {
        get => ReadInt32(100);
        set => WriteInt32(100, value);
    }

    public void Relocated()
    {
        CollisionVolume collisionVolume = CollisionVolume;
        if (collisionVolume != null)
            collisionVolume.Updated = false;
        HasMoved = true;
        HasRelocated = true;
    }

    public void Update()
    {
        if (HasMoved || HasRelocated)
        {
            CollisionVolume?.UpdateAll();
        }
    }

    public void Moved(Vector3 dp)
    {
        HasMoved = true;
        CollisionVolume collisionVolume = CollisionVolume;
        if (collisionVolume != null)
        {
            Vector3 position = Vector3.Add(collisionVolume.Position, dp);
            collisionVolume.Position = position;
            collisionVolume.Updated = false;
        }
    }
}
