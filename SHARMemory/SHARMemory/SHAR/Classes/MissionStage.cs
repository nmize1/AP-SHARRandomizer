using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs.MissionStage;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVMissionStage@@")]
public class MissionStage : EventListener
{
    public const int MAX_CONDITIONS = 8;
    public const int MAX_VEHICLES = 4;
    public const int MAX_WAYPOINTS = 32;
    public const int MAX_CHARACTERS_IN_STAGE = 6;

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

    public enum StageTimeTypes
    {
        NotTimed,
        Add,
        Set,
    }

    public MissionStage(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint LoadingManagerProcessRequestsCallbackVFTableOffset = EventListenerVFTableOffset + sizeof(uint);

    internal const uint ConversationCamNameOffset = LoadingManagerProcessRequestsCallbackVFTableOffset + sizeof(uint);
    public long ConversationCamName
    {
        get => ReadInt64(ConversationCamNameOffset);
        set => WriteInt64(ConversationCamNameOffset, value);
    }

    internal const uint ConversationCamNpcNameOffset = ConversationCamNameOffset + sizeof(long);
    public long ConversationCamNpcName
    {
        get => ReadInt64(ConversationCamNpcNameOffset);
        set => WriteInt64(ConversationCamNpcNameOffset, value);
    }

    internal const uint ConversationCamPcNameOffset = ConversationCamNpcNameOffset + sizeof(long);
    public long ConversationCamPcName
    {
        get => ReadInt64(ConversationCamPcNameOffset);
        set => WriteInt64(ConversationCamPcNameOffset, value);
    }

    internal const uint AmbientPcAnimationsRandomOffset = ConversationCamPcNameOffset + sizeof(long);
    public bool AmbientPcAnimationsRandom
    {
        get => ReadBoolean(AmbientPcAnimationsRandomOffset);
        set => WriteBoolean(AmbientPcAnimationsRandomOffset, value);
    }

    internal const uint AmbientNpcAnimationsRandomOffset = AmbientPcAnimationsRandomOffset + sizeof(bool);
    public bool AmbientNpcAnimationsRandom
    {
        get => ReadBoolean(AmbientNpcAnimationsRandomOffset);
        set => WriteBoolean(AmbientNpcAnimationsRandomOffset, value);
    }

    internal const uint AmbientPcAnimationsOffset = AmbientNpcAnimationsRandomOffset + 3; // Padding
    // TODO: AmbientPcAnimations

    internal const uint AmbientNpcAnimationsOffset = AmbientPcAnimationsOffset + 16;
    // TODO: AmbientNpcAnimations

    internal const uint PcIsChildOffset = AmbientNpcAnimationsOffset + 16;
    public bool PcIsChild
    {
        get => ReadBitfield(PcIsChildOffset, 0);
        set => WriteBitfield(PcIsChildOffset, 0, value);
    }

    internal const uint NpcIsChildOffset = PcIsChildOffset + 0;
    public bool NpcIsChild
    {
        get => ReadBitfield(NpcIsChildOffset, 1);
        set => WriteBitfield(NpcIsChildOffset, 1, value);
    }

    internal const uint GoToPattyAndSelmaScreenWhenDoneOffset = NpcIsChildOffset + 0;
    public bool GoToPattyAndSelmaScreenWhenDone
    {
        get => ReadBitfield(GoToPattyAndSelmaScreenWhenDoneOffset, 2);
        set => WriteBitfield(GoToPattyAndSelmaScreenWhenDoneOffset, 2, value);
    }

    internal const uint CamerasForLinesOfDialogOffset = GoToPattyAndSelmaScreenWhenDoneOffset + 4; // Padding
    // TODO CamerasForLinesOfDialog

    internal const uint BestSideLocatorOffset = CamerasForLinesOfDialogOffset + 16;
    public long BestSideLocator
    {
        get => ReadInt64(BestSideLocatorOffset);
        set => WriteInt64(BestSideLocatorOffset, value);
    }

    internal const uint StayBlackForStageOffset = BestSideLocatorOffset + sizeof(long);
    public bool StayBlackForStage
    {
        get => ReadBoolean(StayBlackForStageOffset);
        set => WriteBoolean(StayBlackForStageOffset, value);
    }

    internal const uint DisablePlayerControlForCountDownOffset = StayBlackForStageOffset + sizeof(bool);
    public bool DisablePlayerControlForCountDown
    {
        get => ReadBoolean(DisablePlayerControlForCountDownOffset);
        set => WriteBoolean(DisablePlayerControlForCountDownOffset, value);
    }

    internal const uint StateOffset = DisablePlayerControlForCountDownOffset + 3; // Padding
    public MissionStageStates State
    {
        get => (MissionStageStates)ReadInt32(StateOffset);
        set => WriteInt32(StateOffset, (int)value);
    }

    internal const uint ObjectiveOffset = StateOffset + sizeof(int);
    public MissionObjective Objective => Memory.ClassFactory.Create<MissionObjective>(ReadUInt32(ObjectiveOffset));

    internal const uint NumConditionsOffset = ObjectiveOffset + sizeof(uint);
    public int NumConditions
    {
        get => ReadInt32(NumConditionsOffset);
        set => WriteInt32(NumConditionsOffset, value);
    }

    internal const uint ConditionsOffset = NumConditionsOffset + sizeof(int);
    public PointerArray<MissionCondition> Conditions => new(Memory, Address + ConditionsOffset, NumConditions);

    internal const uint StageTimeTypeOffset = ConditionsOffset + sizeof(uint) * MAX_CONDITIONS;
    public StageTimeTypes StageTimeType
    {
        get => (StageTimeTypes)ReadUInt32(StageTimeTypeOffset);
        set => WriteUInt32(StageTimeTypeOffset, (uint)value);
    }

    internal const uint StageTimeOffset = StageTimeTypeOffset + sizeof(uint);
    public int StageTime
    {
        get => ReadInt32(StageTimeOffset);
        set => WriteInt32(StageTimeOffset, value);
    }

    internal const uint NumVehiclesOffset = StageTimeOffset + sizeof(int);
    public int NumVehicles
    {
        get => ReadInt32(NumVehiclesOffset);
        set => WriteInt32(NumVehiclesOffset, value);
    }

    internal const uint VehiclesOffset = NumVehiclesOffset + sizeof(int);
    public StructArray<VehicleInfo> Vehicles => new(Memory, Address + VehiclesOffset, VehicleInfo.Size, MAX_VEHICLES);

    internal const uint NumWaypointsOffset = VehiclesOffset + VehicleInfo.Size * MAX_VEHICLES;
    public int NumWaypoints
    {
        get => ReadInt32(NumWaypointsOffset);
        set => WriteInt32(NumWaypointsOffset, value);
    }

    internal const uint WaypointsOffset = NumWaypointsOffset + sizeof(int);
    public PointerArray<Locator> Waypoints => new(Memory, Address + WaypointsOffset, NumWaypoints);

    internal const uint NumCharactersOffset = WaypointsOffset + sizeof(uint) * MAX_WAYPOINTS;
    public int NumCharacters
    {
        get => ReadInt32(NumCharactersOffset);
        set => WriteInt32(NumCharactersOffset, value);
    }

    internal const uint CharactersOffset = NumCharactersOffset + sizeof(int);
    public StructArray<CharacterInfo> Characters => new(Memory, Address + CharactersOffset, CharacterInfo.Size, MAX_CHARACTERS_IN_STAGE);

    internal const uint CharacterToHideOffset = CharactersOffset + CharacterInfo.Size * MAX_CHARACTERS_IN_STAGE;
    public string CharacterToHide
    {
        get => ReadString(CharacterToHideOffset, System.Text.Encoding.UTF8, 16);
        set => WriteString(CharacterToHideOffset, value, System.Text.Encoding.UTF8, 16);
    }

    internal const uint LevelOverOffset = CharacterToHideOffset + 16;
    public bool LevelOver
    {
        get => ReadBitfield(LevelOverOffset, 0);
        set => WriteBitfield(LevelOverOffset, 0, value);
    }

    internal const uint GameOverOffset = LevelOverOffset + 0;
    public bool GameOver
    {
        get => ReadBitfield(GameOverOffset, 1);
        set => WriteBitfield(GameOverOffset, 1, value);
    }

    internal const uint FinalStageOffset = GameOverOffset + 0;
    public bool FinalStage
    {
        get => ReadBitfield(FinalStageOffset, 2);
        set => WriteBitfield(FinalStageOffset, 2, value);
    }

    internal const uint NameIndexOffset = FinalStageOffset + 4; // Padding
    public int NameIndex
    {
        get => ReadInt32(NameIndexOffset);
        set => WriteInt32(NameIndexOffset, value);
    }

    public MissionStage Clone()
    {
        var address = Memory.AllocateAndWriteMemory(ReadBytes(0, 0x468));
        return Memory.ClassFactory.Create<MissionStage>(address);
    }
}
