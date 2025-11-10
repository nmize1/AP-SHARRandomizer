using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVIGuiScreenRewards@@")]
public class IGuiScreenRewards : CGuiScreen
{
    public const int MAX_NUM_PREVIEW_VEHICLES = 64;
    public const int MAX_NUM_PREVIEW_CLOTHING = 4;
    public const int NUM_VEHICLE_RATINGS = 4;

    public enum ScreenState
    {
        OpeningLight,
        LightOpened,
        ClosingLight,
        LightClosed,
    }

    public enum VehicleRatingType
    {
        Speed,
        Acceleration,
        Toughness,
        Stability,
    }
    
    public IGuiScreenRewards(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint LoadingManagerProcessRequestsCallbackVFTableOffset = PlayTransitionAnimationLastOffset + 4; // Padding

    internal const uint RewardsMenuOffset = LoadingManagerProcessRequestsCallbackVFTableOffset + sizeof(uint);

    internal const uint CurrentStateOffset = RewardsMenuOffset + sizeof(uint);
    public ScreenState CurrentState
    {
        get => (ScreenState)ReadInt32(CurrentStateOffset);
        set => WriteInt32(CurrentStateOffset, (int)value);
    }

    internal const uint ElapsedTimeOffset = CurrentStateOffset + sizeof(int);
    public float ElapsedTime
    {
        get => ReadSingle(ElapsedTimeOffset);
        set => WriteSingle(ElapsedTimeOffset, value);
    }

    internal const uint PreviewLightCoverOffset = ElapsedTimeOffset + sizeof(float);
    public FeEntity PreviewLightCover => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(PreviewLightCoverOffset));

    internal const uint PreviewWindowOffset = PreviewLightCoverOffset + sizeof(uint);
    public FeEntity PreviewWindow => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(PreviewWindowOffset));

    internal const uint PreviewPedestalOffset = PreviewWindowOffset + sizeof(uint);
    public FeEntity PreviewPedestal => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(PreviewPedestalOffset));

    internal const uint PreviewBgdOffset = PreviewPedestalOffset + sizeof(uint);
    public FeEntity PreviewBgd => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(PreviewBgdOffset));

    internal const uint PreviewImageOffset = PreviewBgdOffset + sizeof(uint);
    public FeEntity PreviewImage => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(PreviewImageOffset));

    internal const uint PreviewNameOffset = PreviewImageOffset + sizeof(uint);
    public FeEntity PreviewName => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(PreviewNameOffset));

    internal const uint PreviewVehiclesOffset = PreviewNameOffset + sizeof(uint);
    public StructArray<PreviewObject> PreviewVehicles => new(Memory, Address + PreviewVehiclesOffset, PreviewObject.Size, MAX_NUM_PREVIEW_VEHICLES);

    internal const uint NumPreviewVehiclesOffset = PreviewVehiclesOffset + PreviewObject.Size * MAX_NUM_PREVIEW_VEHICLES;
    public int NumPreviewVehicles
    {
        get => ReadInt32(NumPreviewVehiclesOffset);
        set => WriteInt32(NumPreviewVehiclesOffset, value);
    }

    internal const uint CurrentPreviewVehicleOffset = NumPreviewVehiclesOffset + sizeof(int);
    public int CurrentPreviewVehicle
    {
        get => ReadInt32(CurrentPreviewVehicleOffset);
        set => WriteInt32(CurrentPreviewVehicleOffset, value);
    }

    internal const uint PreviewClothingOffset = CurrentPreviewVehicleOffset + sizeof(int);
    public StructArray<PreviewObject> PreviewClothing => new(Memory, Address + PreviewClothingOffset, PreviewObject.Size, MAX_NUM_PREVIEW_CLOTHING);

    internal const uint NumPreviewClothingOffset = PreviewClothingOffset + PreviewObject.Size * MAX_NUM_PREVIEW_CLOTHING;
    public int NumPreviewClothing
    {
        get => ReadInt32(NumPreviewClothingOffset);
        set => WriteInt32(NumPreviewClothingOffset, value);
    }

    internal const uint CurrentPreviewClothingOffset = NumPreviewClothingOffset + sizeof(int);
    public int CurrentPreviewClothing
    {
        get => ReadInt32(CurrentPreviewClothingOffset);
        set => WriteInt32(CurrentPreviewClothingOffset, value);
    }

    internal const uint Bitfield_0x2D70OFfset = CurrentPreviewClothingOffset + sizeof(int);
    private byte Bitfield_0x2D70
    {
        get => ReadByte(Bitfield_0x2D70OFfset);
        set => WriteByte(Bitfield_0x2D70OFfset, value);
    }

    public bool IsLoading
    {
        get => (Bitfield_0x2D70 & 0b00000001) != 0;
        set
        {
            if (value)
                Bitfield_0x2D70 |= 0b00000001;
            else
                Bitfield_0x2D70 &= 0b11111110;
        }
    }

    public bool IsLoadingReward
    {
        get => (Bitfield_0x2D70 & 0b00000010) != 0;
        set
        {
            if (value)
                Bitfield_0x2D70 |= 0b00000010;
            else
                Bitfield_0x2D70 &= 0b11111101;
        }
    }

    internal const uint LockedOverlayOffset = Bitfield_0x2D70OFfset + 4; // Padding
    public FeEntity LockedOverlay => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(LockedOverlayOffset));

    internal const uint LockedLevelOffset = LockedOverlayOffset + sizeof(uint);
    public FeEntity LockedLevel => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(LockedLevelOffset));

    internal const uint RewardPriceOffset = LockedLevelOffset + sizeof(uint);
    public FeEntity RewardPrice => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(RewardPriceOffset));

    internal const uint StatsOverlayOffset = RewardPriceOffset + sizeof(uint);
    public FeEntity StatsOverlay => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(StatsOverlayOffset));

    internal const uint StatsOverlayButtonOffset = StatsOverlayOffset + sizeof(uint);
    public FeEntity StatsOverlayButton => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(StatsOverlayButtonOffset));

    internal const uint StatsOverlayButtonLabelOffset = StatsOverlayButtonOffset + sizeof(uint);
    public FeEntity StatsOverlayButtonLabel => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(StatsOverlayButtonLabelOffset));

    internal const uint StatsOverlayToggleOffset = StatsOverlayButtonLabelOffset + sizeof(uint);
    public bool StatsOverlayToggle
    {
        get => ReadBoolean(StatsOverlayToggleOffset);
        set => WriteBoolean(StatsOverlayToggleOffset, value);
    }

    internal const uint VehicleRatingsOffset = StatsOverlayToggleOffset + 4; // Padding
}
