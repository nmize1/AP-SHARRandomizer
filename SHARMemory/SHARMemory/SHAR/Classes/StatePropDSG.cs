using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVStatePropDSG@@")]
public class StatePropDSG : DynaPhysDSG
{
    public StatePropDSG(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint CStatePropListenerVFTableOffset = GroundPlaneRefsOffset + sizeof(int);

    internal const uint TransformOffset = CStatePropListenerVFTableOffset + sizeof(uint);
    public Matrix4x4 Transform
    {
        get => ReadStruct<Matrix4x4>(TransformOffset);
        set => WriteStruct(TransformOffset, value);
    }

    internal const uint StatePropOffset = TransformOffset + Matrix4x4.Size;
    public CStateProp StateProp => Memory.ClassFactory.Create<CStateProp>(ReadUInt32(StatePropOffset));

    internal const uint PhysObjOffset = StatePropOffset + sizeof(uint);
    public PhysicsObject PhysObj => Memory.ClassFactory.Create<PhysicsObject>(ReadUInt32(PhysObjOffset));

    internal const uint IsDynaLoadedOffset = PhysObjOffset + sizeof(uint);
    public bool IsDynaLoaded
    {
        get => ReadBoolean(IsDynaLoadedOffset);
        set => WriteBoolean(IsDynaLoadedOffset, value);
    }

    internal const uint ProcAnimatorOffset = IsDynaLoadedOffset + 4; // Padding
    public StatePropDSGProcAnimator ProcAnimator => Memory.ClassFactory.Create<StatePropDSGProcAnimator>(ReadUInt32(ProcAnimatorOffset));

    internal const uint ShadowElementOffset = ProcAnimatorOffset + sizeof(int);
    public int ShadowElement
    {
        get => ReadInt32(ShadowElementOffset);
        set => WriteInt32(ShadowElementOffset, value);
    }

    internal const uint SimAnimJointOffset = ShadowElementOffset + sizeof(int);
    public int SimAnimJoint
    {
        get => ReadInt32(SimAnimJointOffset);
        set => WriteInt32(SimAnimJointOffset, value);
    }
}
