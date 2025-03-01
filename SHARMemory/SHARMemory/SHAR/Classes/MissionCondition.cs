using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVMissionCondition@@")]
public class MissionCondition : Class
{
    public enum ConditionTypes
    {
        Invalid,
        VehicleDamage,
        PlayerHit,
        TimeOut,
        PlayerOutOfVehicle,
        FollowDistance,
        OutOfBounds,
        Race,
        LeaveInterior,
        Position,
        CarryingStatepropCollectible,
        NotAbducted,
        HitAndRunCaught,
        KeepBarrel,
        GetCollectibles,
        NumConditions
    }

    public MissionCondition(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint MissionConditionVFTableOffset = 0;

    internal const uint TypeOffset = MissionConditionVFTableOffset + sizeof(uint);
    public ConditionTypes Type
    {
        get => (ConditionTypes)ReadInt32(TypeOffset);
        set => WriteInt32(TypeOffset, (int)value);
    }

    internal const uint IsViolatedOffset = TypeOffset + sizeof(uint);
    public bool IsViolated
    {
        get => ReadBoolean(IsViolatedOffset);
        set => WriteBoolean(IsViolatedOffset, value);
    }

    internal const uint LeaveInteriorOffset = IsViolatedOffset + 1;
    public bool LeaveInterior
    {
        get => ReadBoolean(IsViolatedOffset);
        set => WriteBoolean(IsViolatedOffset, value);
    }
}
