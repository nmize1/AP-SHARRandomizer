using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System.Text;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVGameplayManager@@")]
public class GameplayManager : Class
{
    public const int MAX_MISSIONS = 20;
    public const int MAX_BONUS_MISSIONS = 10;
    public const int MAX_VDU_CARS = 10;

    public enum CarSlots
    {
        DefaultCar,
        OtherCar,
        AICar
    }

    public enum GameTypes
    {
        Normal,
        SuperSprint, // Bonus Game
        Num_GameTypes
    }

    public enum Messages
    {
        None,
        NextMission,
        PrevMission
    }

    public enum Ratings
    {
        DNF,
        Bronze,
        Silver,
        Gold,
        Num_Ratins
    }

    public GameplayManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint EventListenerVFTableOffset = 0;
    internal const uint PresentationEventCallBackVFTableOffset = EventListenerVFTableOffset + sizeof(uint);

    internal const uint IsDemoOffset = PresentationEventCallBackVFTableOffset + sizeof(uint);
    public bool IsDemo
    {
        get => ReadBoolean(IsDemoOffset);
        set => WriteBoolean(IsDemoOffset, value);
    }

    internal const uint PlayerAndCarInfoOffset = IsDemoOffset + 4; // Padding
    public PlayerAndCarInfo PlayerAndCarInfo
    {
        get => ReadStruct<PlayerAndCarInfo>(PlayerAndCarInfoOffset);
        set => WriteStruct(PlayerAndCarInfoOffset, value);
    }

    internal const uint CharacterIndexOffset = PlayerAndCarInfoOffset + PlayerAndCarInfo.Size;
    public int CharacterIndex
    {
        get => ReadInt32(CharacterIndexOffset);
        set => WriteInt32(CharacterIndexOffset, value);
    }

    internal const uint VehicleSlotsOffset = CharacterIndexOffset + sizeof(int);
    public StructArray<CarData> VehicleSlots => new(Memory, ReadUInt32(VehicleSlotsOffset), CarData.Size, 3);

    internal const uint DefaultLevelVehicleNameOffset = VehicleSlotsOffset + CarData.Size * 3;
    public string DefaultLevelVehicleName
    {
        get => ReadString(DefaultLevelVehicleNameOffset, Encoding.UTF8, 64);
        set => WriteString(DefaultLevelVehicleNameOffset, value, Encoding.UTF8, 64);
    }

    internal const uint DefaultLevelVehicleLocatorOffset = DefaultLevelVehicleNameOffset + 64;
    public string DefaultLevelVehicleLocator
    {
        get => ReadString(DefaultLevelVehicleLocatorOffset, Encoding.UTF8, 64);
        set => WriteString(DefaultLevelVehicleLocatorOffset, value, Encoding.UTF8, 64);
    }

    internal const uint DefaultLevelVehicleConfigOffset = DefaultLevelVehicleLocatorOffset + 64;
    public string DefaultLevelVehicleConfig
    {
        get => ReadString(DefaultLevelVehicleConfigOffset, Encoding.UTF8, 64);
        set => WriteString(DefaultLevelVehicleConfigOffset, value, Encoding.UTF8, 64);
    }

    internal const uint ShouldLoadDefaultVehicleOffset = DefaultLevelVehicleConfigOffset + 64;
    public bool ShouldLoadDefaultVehicle
    {
        get => ReadBoolean(ShouldLoadDefaultVehicleOffset);
        set => WriteBoolean(ShouldLoadDefaultVehicleOffset, value);
    }

    internal const uint MissionVehicleSlotsOffset = ShouldLoadDefaultVehicleOffset + 4; // Padding
    public StructArray<MissionCarData> MissionVehicleSlots => new(Memory, ReadUInt32(MissionVehicleSlotsOffset), MissionCarData.Size, 5);

    internal const uint ChaseManagersOffset = MissionVehicleSlotsOffset + MissionCarData.Size * 5;
    public StructArray<Structs.ChaseManager> ChaseManagers => new(Memory, ReadUInt32(ChaseManagersOffset), Structs.ChaseManager.Size, 1);

    internal const uint BlackScreenTimerOffset = ChaseManagersOffset + Structs.ChaseManager.Size * 1;
    public int BlackScreenTimer
    {
        get => ReadInt32(BlackScreenTimerOffset);
        set => WriteInt32(BlackScreenTimerOffset, value);
    }

    internal const uint SkipSundayOffset = BlackScreenTimerOffset + sizeof(int);
    public bool SkipSunday
    {
        get => ReadBoolean(SkipSundayOffset);
        set => WriteBoolean(SkipSundayOffset, value);
    }

    internal const uint AIIndexOffset = SkipSundayOffset + 4; // Padding
    public int AIIndex
    {
        get => ReadInt32(AIIndexOffset);
        set => WriteInt32(AIIndexOffset, value);
    }

    internal const uint GameTypeOffset = AIIndexOffset + sizeof(int);
    public GameTypes GameType
    {
        get => (GameTypes)ReadUInt32(GameTypeOffset);
        set => WriteUInt32(GameTypeOffset, (uint)value);
    }

    internal const uint PostLevelFMVOffset = GameTypeOffset + sizeof(uint);
    public string PostLevelFMV
    {
        get => ReadString(PostLevelFMVOffset, Encoding.UTF8, 13);
        set => WriteString(PostLevelFMVOffset, value, Encoding.UTF8, 13);
    }

    internal const uint IrisClosedOffset = PostLevelFMVOffset + 13;
    public bool IrisClosed
    {
        get => ReadBitfield(IrisClosedOffset, 0);
        set => WriteBitfield(IrisClosedOffset, 0, value);
    }

    internal const uint FadedToBlackOffset = IrisClosedOffset + 0;
    public bool FadedToBlack
    {
        get => ReadBitfield(FadedToBlackOffset, 1);
        set => WriteBitfield(FadedToBlackOffset, 1, value);
    }

    internal const uint WaitingOnFMVOffset = FadedToBlackOffset + 0;
    public bool WaitingOnFMV
    {
        get => ReadBitfield(WaitingOnFMVOffset, 2);
        set => WriteBitfield(WaitingOnFMVOffset, 2, value);
    }

    internal const uint CurrentMissionOffset = WaitingOnFMVOffset + 3; // Padding
    public int CurrentMission
    {
        get => ReadInt32(CurrentMissionOffset);
        set => WriteInt32(CurrentMissionOffset, value);
    }

    internal const uint NumPlayersOffset = CurrentMissionOffset + sizeof(int);
    public int NumPlayers
    {
        get => ReadInt32(NumPlayersOffset);
        set => WriteInt32(NumPlayersOffset, value);
    }

    internal const uint LevelDataOffset = NumPlayersOffset + sizeof(int);
    public LevelData LevelData
    {
        get => ReadStruct<LevelData>(LevelDataOffset);
        set => WriteStruct(LevelDataOffset, value);
    }

    internal const uint NumMissionsOffset = LevelDataOffset + LevelData.Size;
    public int NumMissions
    {
        get => ReadInt32(NumMissionsOffset);
        set => WriteInt32(NumMissionsOffset, value);
    }

    internal const uint MissionsOffset = NumMissionsOffset + sizeof(int);
    public PointerArray<Mission> Missions => new(Memory, Address + MissionsOffset, MAX_MISSIONS + MAX_BONUS_MISSIONS);

    internal const uint CurrentMissionHeapOffset = MissionsOffset + sizeof(uint) * (MAX_MISSIONS + MAX_BONUS_MISSIONS);
    public uint CurrentMissionHeap // TODO: Add GameMemoryAllocator enum
    {
        get => ReadUInt32(CurrentMissionHeapOffset);
        set => WriteUInt32(CurrentMissionHeapOffset, value);
    }


    internal const uint LevelCompleteOffset = CurrentMissionHeapOffset + sizeof(uint);
    public bool LevelComplete
    {
        get => ReadBitfield(LevelCompleteOffset, 0);
        set => WriteBitfield(LevelCompleteOffset, 0, value);
    }

    internal const uint EnablePhoneBoothsOffset = LevelCompleteOffset + 0;
    public bool EnablePhoneBooths
    {
        get => ReadBitfield(EnablePhoneBoothsOffset, 1);
        set => WriteBitfield(EnablePhoneBoothsOffset, 1, value);
    }

    internal const uint GameCompleteOffset = EnablePhoneBoothsOffset + 0;
    public bool GameComplete
    {
        get => ReadBitfield(GameCompleteOffset, 2);
        set => WriteBitfield(GameCompleteOffset, 2, value);
    }

    internal const uint CurrentVehicleOffset = GameCompleteOffset + 4; // Padding
    public Vehicle CurrentVehicle => Memory.ClassFactory.Create<Vehicle>(ReadUInt32(CurrentVehicleOffset));

    internal const uint VDUOffset = CurrentVehicleOffset + sizeof(uint);
    public VDU VDU
    {
        get => ReadStruct<VDU>(VDUOffset);
        set => WriteStruct(VDUOffset, value);
    }

    internal const uint NumBonusMissionsOffset = VDUOffset + VDU.Size;
    public int NumBonusMissions
    {
        get => ReadInt32(NumBonusMissionsOffset);
        set => WriteInt32(NumBonusMissionsOffset, value);
    }

    internal const uint CurrentBonusMissionOffset = NumBonusMissionsOffset + sizeof(int);
    public int CurrentBonusMission
    {
        get => ReadInt32(CurrentBonusMissionOffset);
        set => WriteInt32(CurrentBonusMissionOffset, value);
    }

    internal const uint DesiredBonusMissionOffset = CurrentBonusMissionOffset + sizeof(int);
    public int DesiredBonusMission
    {
        get => ReadInt32(DesiredBonusMissionOffset);
        set => WriteInt32(DesiredBonusMissionOffset, value);
    }

    internal const uint IsInBonusMissionOffset = DesiredBonusMissionOffset + sizeof(int);
    public bool IsInBonusMission
    {
        get => ReadBoolean(IsInBonusMissionOffset);
        set => WriteBoolean(IsInBonusMissionOffset, value);
    }

    internal const uint FireBonusMissionDialogueOffset = IsInBonusMissionOffset + 1;
    public bool FireBonusMissionDialogue
    {
        get => ReadBoolean(FireBonusMissionDialogueOffset);
        set => WriteBoolean(FireBonusMissionDialogueOffset, value);
    }

    internal const uint JumpToBonusMissionOffset = FireBonusMissionDialogueOffset + 1;
    public bool JumpToBonusMission
    {
        get => ReadBoolean(JumpToBonusMissionOffset);
        set => WriteBoolean(JumpToBonusMissionOffset, value);
    }

    internal const uint UpdateBonusMissionsOffset = JumpToBonusMissionOffset + 1;
    public bool UpdateBonusMissions
    {
        get => ReadBoolean(UpdateBonusMissionsOffset);
        set => WriteBoolean(UpdateBonusMissionsOffset, value);
    }

    internal const uint BonusMissionsOffset = UpdateBonusMissionsOffset + 1;
    public ClassArray<BonusMissionInfo> BonusMissions => new(Memory, Address + BonusMissionsOffset, BonusMissionInfo.Size, MAX_BONUS_MISSIONS);

    internal const uint CurrentMessageOffset = BonusMissionsOffset + BonusMissionInfo.Size * MAX_BONUS_MISSIONS;
    public Messages CurrentMessage
    {
        get => (Messages)ReadInt32(CurrentMessageOffset);
        set => WriteInt32(CurrentMessageOffset, (int)value);
    }

    internal const uint RespawnManagerOffset = CurrentMessageOffset + sizeof(int);
    public RespawnManager RespawnManager => Memory.ClassFactory.Create<RespawnManager>(ReadUInt32(RespawnManagerOffset));

    internal const uint IrisSpeedOffset = RespawnManagerOffset + sizeof(uint);
    public float IrisSpeed
    {
        get => ReadSingle(IrisSpeedOffset);
        set => WriteSingle(IrisSpeedOffset, value);
    }

    internal const uint PutPlayerInCarOffset = IrisSpeedOffset + sizeof(float);
    public bool PutPlayerInCar
    {
        get => ReadBoolean(PutPlayerInCarOffset);
        set => WriteBoolean(PutPlayerInCarOffset, value);
    }

    internal const uint ManualControlFadeOffset = PutPlayerInCarOffset + 1;
    public bool ManualControlFade
    {
        get => ReadBoolean(ManualControlFadeOffset);
        set => WriteBoolean(ManualControlFadeOffset, value);
    }

    internal const uint CurrentVehicleIconIDOffset = ManualControlFadeOffset + 3; // Padding
    public int CurrentVehicleIconID
    {
        get => ReadInt32(CurrentVehicleIconIDOffset);
        set => WriteInt32(CurrentVehicleIconIDOffset, value);
    }

    internal const uint ElapsedIdleTimeOffset = CurrentVehicleIconIDOffset + sizeof(int);
    public uint ElapsedIdleTime
    {
        get => ReadUInt32(ElapsedIdleTimeOffset);
        set => WriteUInt32(ElapsedIdleTimeOffset, value);
    }

    public int GetCurrentMissionIndex()
    {
        if (IsInBonusMission)
        {
            int currBonusMission = CurrentBonusMission;
            if (currBonusMission >= MAX_MISSIONS && currBonusMission < MAX_MISSIONS + MAX_BONUS_MISSIONS)
                return currBonusMission;
        }
        else
        {
            int currMission = CurrentMission;
            if (currMission >= 0 && currMission < NumMissions)
                return currMission;
        }

        return -1;
    }

    public Mission GetCurrentMission()
    {
        int index = GetCurrentMissionIndex();
        return index >= 0 ? Missions[index] : null;
    }

    public void RepairCurrentVehicle()
    {
        Vehicle currVehicle = CurrentVehicle;
        if (currVehicle == null)
            return;

        CarData testVehicleData = VehicleSlots[(int)CarSlots.DefaultCar];
        if (currVehicle == testVehicleData.Vehicle)
        {
            RepairVehicle(CarSlots.DefaultCar, testVehicleData);
        }
        else
        {
            testVehicleData = VehicleSlots[(int)CarSlots.OtherCar];
            if (currVehicle == testVehicleData.Vehicle)
            {
                RepairVehicle(CarSlots.OtherCar, testVehicleData);

                MissionCarData[] missionVehicleSlots = [.. MissionVehicleSlots];
                for (int i = 0; i < missionVehicleSlots.Length; i++)
                {
                    MissionCarData missionCarData = missionVehicleSlots[i];
                    if (missionCarData.Vehicle == testVehicleData.Vehicle)
                    {
                        //missionCarData.HuskVehicle->Release();
                        missionCarData.HuskVehicle = null;
                        missionCarData.UsingHusk = false;
                        missionVehicleSlots[i] = missionCarData;
                    }
                }
                MissionVehicleSlots.FromArray(missionVehicleSlots);
            }
        }
    }

    private void RepairVehicle(CarSlots carSlot, CarData carData)
    {
        if (carSlot == CarSlots.AICar)
            return;

        Vehicle veh = carData.Vehicle;
        if (veh == null)
            return;

        if (carData.UsingHusk)
        {
            Vehicle husk = carData.HuskVehicle;

            Vector3 carPos = husk.Position;
            float angle = (float)husk.GetFacingInRadians();

            Matrix4x4 m = new(0);
            m.Identity();
            m.FillRotateXYZ(0f, angle, 0f);
            m.FillTranslate(carPos);
            veh.SetTransform(m);
        }

        VehicleSlots[(int)carSlot] = carData;
    }
}
