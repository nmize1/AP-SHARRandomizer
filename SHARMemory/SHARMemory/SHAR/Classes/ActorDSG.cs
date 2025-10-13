using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVActorDSG@@")]
public class ActorDSG : StatePropDSG
{
    public ActorDSG(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint IDOffset = SimAnimJointOffset + sizeof(int);
    public uint ID
    {
        get => ReadUInt32(IDOffset);
        set => WriteUInt32(IDOffset, value);
    }

    internal const uint ShieldPropOffset = IDOffset + sizeof(uint);
    public CStateProp ShieldProp => Memory.ClassFactory.Create<CStateProp>(ReadUInt32(ShieldPropOffset));

    internal const uint TractorBeamPropOffset = ShieldPropOffset + sizeof(uint);
    public CStateProp TractorBeamProp => Memory.ClassFactory.Create<CStateProp>(ReadUInt32(TractorBeamPropOffset));

    internal const uint ShieldEnabledOffset = TractorBeamPropOffset + sizeof(uint);
    public bool ShieldEnabled
    {
        get => ReadBoolean(ShieldEnabledOffset);
        set => WriteBoolean(ShieldEnabledOffset, value);
    }

    internal const uint PhysicsPropertiesOFfset = ShieldEnabledOffset + 4; // Padding
    public PhysicsProperties PhysicsProperties => Memory.ClassFactory.Create<PhysicsProperties>(ReadUInt32(PhysicsPropertiesOFfset));
}
