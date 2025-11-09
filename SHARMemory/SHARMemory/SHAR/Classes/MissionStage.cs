using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVMissionStage@@")]
public class MissionStage : EventListener
{
    public enum MissionStageStates
    {
        Idle,
        InProgress,
        Complete,
        Failed,
        Backup,
        AllComplete,
        NumStates
    }

    public MissionStage(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint LoadingManagerProcessRequestsCallbackVFTableOffset = EventListenerVFTableOffset + sizeof(uint);

    public bool StayBlackForStage
    {
        get => ReadBoolean(96);
        set => WriteBoolean(96, value);
    }

    public bool DisablePlayerControlForCountDown
    {
        get => ReadBoolean(97);
        set => WriteBoolean(97, value);
    }

    public MissionStageStates State
    {
        get => (MissionStageStates)ReadInt32(100);
        set => WriteInt32(100, (int)value);
    }

    public MissionObjective Objective => Memory.ClassFactory.Create<MissionObjective>(ReadUInt32(104));

    public int NumConditions
    {
        get => ReadInt32(108);
        set => WriteInt32(108, value);
    }

    public PointerArray<MissionCondition> Conditions => new(Memory, Address + 112, 8);

    public MissionStage Clone()
    {
        var address = Memory.AllocateAndWriteMemory(ReadBytes(0, 0x468));
        return Memory.ClassFactory.Create<MissionStage>(address);
    }
}
