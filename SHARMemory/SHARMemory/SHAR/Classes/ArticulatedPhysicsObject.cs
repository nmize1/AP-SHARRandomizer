using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVArticulatedPhysicsObject@sim@@")]
public class ArticulatedPhysicsObject : PhysicsObject
{
    public ArticulatedPhysicsObject(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public PointerArray<PhysicsJoint> SimJoints => new(Memory, ReadUInt32(646), NumSimJoints);

    public PointerArray<PhysicsJoint> Joints => new(Memory, ReadUInt32(468), NumJoints);

    public int NumSimJoints
    {
        get => ReadInt32(472);
        set => WriteInt32(472, value);
    }

    public int NumJoints
    {
        get => ReadInt32(476);
        set => WriteInt32(476, value);
    }

    public tPose Pose => Memory.ClassFactory.Create<tPose>(ReadUInt32(480));

    public float TimeSinceComputeInertiaMatrix
    {
        get => ReadSingle(484);
        set => WriteSingle(484, value);
    }

    public float TimeComputeInertiaMatrix
    {
        get => ReadSingle(488);
        set => WriteSingle(488, value);
    }

    public PhysicsJoint CollidingJoint => Memory.ClassFactory.Create<PhysicsJoint>(ReadUInt32(492));

    public PhysicsJoint SelfCollisionCenter => Memory.ClassFactory.Create<PhysicsJoint>(ReadUInt32(496));

    public bool SelfCollision
    {
        get => ReadBoolean(500);
        set => WriteBoolean(500, value);
    }

    public bool AllCacheEmpty
    {
        get => ReadBoolean(501);
        set => WriteBoolean(501, value);
    }

    public SkeletonInfo SkeletonInfo => Memory.ClassFactory.Create<SkeletonInfo>(ReadUInt32(504));

    public float TimeStep
    {
        get => ReadSingle(508);
        set => WriteSingle(508, value);
    }

    public float DissipationDeformationRate
    {
        get => ReadSingle(512);
        set => WriteSingle(512, value);
    }

    public float DissipationDeformationSpeedRate
    {
        get => ReadSingle(516);
        set => WriteSingle(516, value);
    }

    public float DissipationInternalRate
    {
        get => ReadSingle(520);
        set => WriteSingle(520, value);
    }

    public bool ImpulseReactionEnabled
    {
        get => ReadBoolean(524);
        set => WriteBoolean(524, value);
    }
}
