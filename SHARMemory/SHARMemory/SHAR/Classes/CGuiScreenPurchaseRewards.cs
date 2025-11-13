using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs.tMultiController;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiScreenPurchaseRewards@@")]
public class CGuiScreenPurchaseRewards : IGuiScreenRewards
{
    public CGuiScreenPurchaseRewards(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint CurrentTypeOffset = VehicleRatingsOffset + sizeof(uint) * NUM_VEHICLE_RATINGS;
    public Merchandise.SellerTypes CurrentType
    {
        get => (Merchandise.SellerTypes)ReadInt32(CurrentTypeOffset);
        set => WriteInt32(CurrentTypeOffset, (int)value);
    }

    internal const uint IsPurchasingRewardOffset = CurrentTypeOffset + sizeof(int);
    public bool IsPurschasingReward
    {
        get => ReadBoolean(IsPurchasingRewardOffset);
        set => WriteBoolean(IsPurchasingRewardOffset, value);
    }

    internal const uint ElapsedCoinDecrementTotalTimeOffset = IsPurchasingRewardOffset + 4; // Padding
    public uint ElapsedCoinDecrementTotalTime
    {
        get => ReadUInt32(ElapsedCoinDecrementTotalTimeOffset);
        set => WriteUInt32(ElapsedCoinDecrementTotalTimeOffset, value);
    }

    internal const uint ElapsedCoinDecrementTimeOffset = ElapsedCoinDecrementTotalTimeOffset + sizeof(uint);
    public uint ElapsedCoinDecrementTime
    {
        get => ReadUInt32(ElapsedCoinDecrementTimeOffset);
        set => WriteUInt32(ElapsedCoinDecrementTimeOffset, value);
    }

    internal const uint BankValueBeforePurchaseOffset = ElapsedCoinDecrementTimeOffset + sizeof(uint);
    public int BankValueBeforePurchase
    {
        get => ReadInt32(BankValueBeforePurchaseOffset);
        set => WriteInt32(BankValueBeforePurchaseOffset, value);
    }

    internal const uint PurchaseLabelOffset = BankValueBeforePurchaseOffset + sizeof(uint);
    public FeEntity PurchaseLabel => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(PurchaseLabelOffset));

    internal const uint LeftArrowOffset = PurchaseLabelOffset + sizeof(uint);
    public FeEntity LeftArrow => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(LeftArrowOffset));

    internal const uint RightArrowOffset = LeftArrowOffset + sizeof(uint);
    public FeEntity RightArrow => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(RightArrowOffset));

    internal const uint SkinTrackInfoOffset = RightArrowOffset + sizeof(uint);
    public TrackInfo SkinTrackInfo
    {
        get => ReadStruct<TrackInfo>(SkinTrackInfoOffset);
        set => WriteStruct(SkinTrackInfoOffset, value);
    }

    internal const uint SkinMultiControllerOffset = SkinTrackInfoOffset + TrackInfo.Size;
    public tMultiController SkinMultiController => Memory.ClassFactory.Create<tMultiController>(ReadUInt32(SkinMultiControllerOffset));

    internal const uint SkinAnimControllerOffset = SkinMultiControllerOffset + sizeof(uint);
}
