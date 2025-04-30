using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System.Text;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVMission@@")]
public class Mission : Class
{
    public enum MissionStates
    {
        Waiting,
        InProgress,
        Failed,
        Success,
        NumStates
    }

    public Mission(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator)
    {
        if (memory.ModLauncherOrdinalAddresses.TryGetValue(Memory.ModLauncherOrdinals.MaxStages, out uint MaxStagesAddress) && memory.ModLauncherOrdinalAddresses.TryGetValue(Memory.ModLauncherOrdinals.StagesOffset, out uint StagesOffsetAddress))
        {
            MaxStages = memory.ReadUInt32(MaxStagesAddress);
            StagesOffset = memory.ReadUInt32(StagesOffsetAddress);
        }
        else
        {
            MaxStages = 25;
            StagesOffset = 12;
        }
    }

    public bool IsStreetRace1Or2
    {
        get => ReadBoolean(4);
        set => WriteBoolean(4, value);
    }

    public int NumMissionStages
    {
        get => ReadInt32(8);
        set => WriteInt32(8, value);
    }

    private readonly uint MaxStages;
    private readonly uint StagesOffset;
    public PointerArray<MissionStage> MissionStages => new(Memory, Address + StagesOffset, (int)MaxStages);

    public int CurrentStage
    {
        get => ReadInt32(112);
        set => WriteInt32(112, value);
    }

    public int ResetMission
    {
        get => ReadInt32(116);
        set => WriteInt32(116, value);
    }

    public string Name
    {
        get => ReadString(120, Encoding.UTF8, 16);
        set => WriteString(120, value, Encoding.UTF8, 16);
    }

    // GameMemoryAllocator CurrentMissionHeap (136)

    public bool Complete
    {
        get => ReadBoolean(140);
        set => WriteBoolean(140, value);
    }

    public bool IsLastStage
    {
        get => ReadBoolean(141);
        set => WriteBoolean(141, value);
    }

    public int MissionTimer
    {
        get => ReadInt32(144);
        set => WriteInt32(144, value);
    }

    public int ElapsedTimeMS
    {
        get => ReadInt32(148);
        set => WriteInt32(148, value);
    }

    public MissionStates State
    {
        get => (MissionStates)ReadInt32(152);
        set => WriteInt32(152, (int)value);
    }

    public MissionStage.MissionStageStates LastStageState
    {
        get => (MissionStage.MissionStageStates)ReadInt32(156);
        set => WriteInt32(156, (int)value);
    }

    public CarStartLocator VehicleRestart => Memory.ClassFactory.Create<CarStartLocator>(ReadUInt32(160));

    public Locator PlayerRestart => Memory.ClassFactory.Create<Locator>(ReadUInt32(164));

    public ZoneEventLocator DynaloadLoc => Memory.ClassFactory.Create<ZoneEventLocator>(ReadUInt32(168));

    public ZoneEventLocator StreetRacePropsLoad => Memory.ClassFactory.Create<ZoneEventLocator>(ReadUInt32(172));

    public ZoneEventLocator StreetRacePropsUnload => Memory.ClassFactory.Create<ZoneEventLocator>(ReadUInt32(176));

    public int ResetToStage
    {
        get => ReadInt32(180);
        set => WriteInt32(180, value);
    }

    private byte Bitfield_0xB8
    {
        get => ReadByte(184);
        set => WriteByte(184, value);
    }

    public bool SundayDrive
    {
        get => (Bitfield_0xB8 & 0b00000001) != 0;
        set
        {
            if (value)
                Bitfield_0xB8 |= 0b00000001;
            else
                Bitfield_0xB8 &= 0b11111110;
        }
    }

    public bool BonusMission
    {
        get => (Bitfield_0xB8 & 0b00000010) != 0;
        set
        {
            if (value)
                Bitfield_0xB8 |= 0b00000010;
            else
                Bitfield_0xB8 &= 0b11111101;
        }
    }

    public PointerArray<BonusObjective> BonusObjectives => new(Memory, Address + 188, 3);

    public uint NumBonusObjectives
    {
        get => ReadUInt32(192);
        set => WriteUInt32(192, value);
    }

    public bool IsForcedCar
    {
        get => ReadBoolean(196);
        set => WriteBoolean(196, value);
    }

    public bool AutoRepairCar
    {
        get => ReadBoolean(197);
        set => WriteBoolean(197, value);
    }

    public bool SwappedCars
    {
        get => ReadBoolean(198);
        set => WriteBoolean(198, value);
    }

    public bool CarryOverOutOfCarCondition
    {
        get => ReadBoolean(199);
        set => WriteBoolean(199, value);
    }

    public bool TriggerPattyAndSelmaScreen
    {
        get => ReadBoolean(200);
        set => WriteBoolean(200, value);
    }

    public int FinalDelay
    {
        get => ReadInt32(204);
        set => WriteInt32(204, value);
    }

    public int CompleteDelay
    {
        get => ReadInt32(208);
        set => WriteInt32(208, value);
    }

    public bool ChangingStages
    {
        get => ReadBoolean(212);
        set => WriteBoolean(212, value);
    }

    public bool NoTimeUpdate
    {
        get => ReadBoolean(213);
        set => WriteBoolean(213, value);
    }

    public bool JumpBackStage
    {
        get => ReadBoolean(214);
        set => WriteBoolean(214, value);
    }

    public byte JumpBackStageBy
    {
        get => ReadByte(215);
        set => WriteByte(215, value);
    }

    public int NumStatePropCollectibles
    {
        get => ReadInt32(216);
        set => WriteInt32(216, value);
    }

    public PointerArray<StatePropCollectible> StatePropCollectibles => new(Memory, Address + 220, NumStatePropCollectibles);

    public AnimatedIcon DoorStars => Memory.ClassFactory.Create<AnimatedIcon>(ReadUInt32(224));

    public int InitPedGroupId
    {
        get => ReadInt32(228);
        set => WriteInt32(228, value);
    }

    public bool ShowHUD
    {
        get => ReadBoolean(232);
        set => WriteBoolean(232, value);
    }

    public int NumValidFailureHints
    {
        get => ReadInt32(236);
        set => WriteInt32(236, value);
    }

    public MissionStage GetCurrentStage()
    {
        int currStage = CurrentStage;
        if (currStage >= 0 && currStage < NumMissionStages)
            return MissionStages[currStage];
        return null;
    }
}
