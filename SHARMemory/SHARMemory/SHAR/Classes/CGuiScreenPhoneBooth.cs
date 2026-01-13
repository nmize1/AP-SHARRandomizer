using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCGuiScreenPhoneBooth@@")]
public class CGuiScreenPhoneBooth : IGuiScreenRewards
{
    public CGuiScreenPhoneBooth(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint DamagedInfoOffset = VehicleRatingsOffset + sizeof(uint) * NUM_VEHICLE_RATINGS;
    public FeEntity DamagedInfo => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(DamagedInfoOffset));

    internal const uint VehicleDamagedOffset = DamagedInfoOffset + sizeof(uint);
    public FeEntity VehicleDamaged => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(VehicleDamagedOffset));

    internal const uint RepairCostInfoOffset = VehicleDamagedOffset + sizeof(uint);
    public FeEntity RepairCostInfo => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(RepairCostInfoOffset));

    internal const uint VehicleRepairCostOffset = RepairCostInfoOffset + sizeof(uint);
    public FeEntity VehicleRepairCost => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(VehicleRepairCostOffset));

    internal const uint UnknownOffset = VehicleRepairCostOffset + sizeof(uint);
    public FeEntity Husk => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(UnknownOffset));

    internal const uint LeftArrowOffset = UnknownOffset + sizeof(uint);
    public FeEntity LeftArrow => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(LeftArrowOffset));

    internal const uint RightArrowOffset = LeftArrowOffset + sizeof(uint);
    public FeEntity RightArrow => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(RightArrowOffset));

    internal const uint MenuOffset = RightArrowOffset + sizeof(uint);
    public CGuiMenu Menu => Memory.ClassFactory.Create<CGuiMenu>(ReadUInt32(MenuOffset));

    internal const uint CarSelectOverlayOffset = MenuOffset + sizeof(uint);
    public FeEntity CarSelectOverlay => Memory.ClassFactory.Create<FeEntity>(ReadUInt32(CarSelectOverlayOffset));

    internal const uint MenuTeleportOffset = CarSelectOverlayOffset + sizeof(uint);
    public bool MenuTeleport
    {
        get => ReadBoolean(MenuTeleportOffset);
        set => WriteBoolean(MenuTeleportOffset, value);
    }
}
