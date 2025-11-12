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
    
    public IGuiScreenRewards(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator)
    {
        if (memory.ModLauncherOrdinalAddresses.TryGetValue(Memory.ModLauncherOrdinals.GUIScreenRewards_MaxCarPreviews, out uint MaxCarPreviewsAddress) && memory.ModLauncherOrdinalAddresses.TryGetValue(Memory.ModLauncherOrdinals.GUIScreenRewards_CarPreviewsOffset, out uint CarPreviewsOffsetAddress))
        {
            _maxPreviewVehicles = memory.ReadInt32(MaxCarPreviewsAddress);
            _previewVehiclesOffset = memory.ReadUInt32(CarPreviewsOffsetAddress);
        }
        else
        {
            _maxPreviewVehicles = MAX_NUM_PREVIEW_VEHICLES;
            _previewVehiclesOffset = PreviewVehiclesOffset;
        }

        if (memory.ModLauncherOrdinalAddresses.TryGetValue(Memory.ModLauncherOrdinals.GUIScreenRewards_MaxSkinPreviews, out uint MaxSkinPreviewsAddress) && memory.ModLauncherOrdinalAddresses.TryGetValue(Memory.ModLauncherOrdinals.GUIScreenRewards_SkinPreviewsOffset, out uint SkinPreviewsOffsetAddress))
        {
            _maxPreviewClothing = memory.ReadInt32(MaxSkinPreviewsAddress);
            _previewClothingOffset = memory.ReadUInt32(SkinPreviewsOffsetAddress);
        }
        else
        {
            _maxPreviewClothing = MAX_NUM_PREVIEW_CLOTHING;
            _previewClothingOffset = PreviewClothingOffset;
        }
    }

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
    private readonly uint _previewVehiclesOffset;
    private readonly int _maxPreviewVehicles;
    public StructArray<PreviewObject> PreviewVehicles => new(Memory, Address + _previewVehiclesOffset, PreviewObject.Size, _maxPreviewVehicles);

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
    private readonly uint _previewClothingOffset;
    private readonly int _maxPreviewClothing;
    public StructArray<PreviewObject> PreviewClothing => new(Memory, Address + _previewClothingOffset, PreviewObject.Size, _maxPreviewClothing);

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

    internal const uint IsLoadingOffset = CurrentPreviewClothingOffset + sizeof(int);
    public bool IsLoading
    {
        get => ReadBitfield(IsLoadingOffset, 0);
        set => WriteBitfield(IsLoadingOffset, 0, value);
    }

    internal const uint IsLoadingRewardOffset = IsLoadingOffset + 0;
    public bool IsLoadingReward
    {
        get => ReadBitfield(IsLoadingRewardOffset, 1);
        set => WriteBitfield(IsLoadingRewardOffset, 1, value);
    }

    internal const uint LockedOverlayOffset = IsLoadingRewardOffset + 4; // Padding
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
    public PointerArray<PhoneBoothStars> VehicleRatings => new(Memory, Address + VehicleRatingsOffset, NUM_VEHICLE_RATINGS);
}
