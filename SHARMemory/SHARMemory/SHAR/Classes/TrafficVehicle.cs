using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVTrafficVehicle@@")]
public class TrafficVehicle : Class
{
    public TrafficVehicle(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint TrafficVehicleVFTableOffset = 0;

    internal const uint MillisecondsDeactivatedOffset = TrafficVehicleVFTableOffset + sizeof(uint);
    public uint MillisecondsDeactivated
    {
        get => ReadUInt32(MillisecondsDeactivatedOffset);
        set => WriteUInt32(MillisecondsDeactivatedOffset, value);
    }

    internal const uint CanBeResurrectedOffset = MillisecondsDeactivatedOffset + sizeof(uint);
    public bool CanBeResurrected
    {
        get => ReadBoolean(CanBeResurrectedOffset);
        set => WriteBoolean(CanBeResurrectedOffset, value);
    }

    internal const uint MillisecondsOutOfSightOffset = CanBeResurrectedOffset + 4; // Padding
    public uint MillisecondsOutOfSight
    {
        get => ReadUInt32(MillisecondsOutOfSightOffset);
        set => WriteUInt32(MillisecondsOutOfSightOffset, value);
    }

    internal const uint OutOfSightOffset = MillisecondsOutOfSightOffset + sizeof(uint);
    public bool OutOfSight
    {
        get => ReadBoolean(OutOfSightOffset);
        set => WriteBoolean(OutOfSightOffset, value);
    }

    internal const uint VehicleOffset = OutOfSightOffset + 4; // Padding
    public Vehicle Vehicle => Memory.ClassFactory.Create<Vehicle>(ReadUInt32(VehicleOffset));

    internal const uint HuskOffset = VehicleOffset + sizeof(uint);
    public Vehicle Husk => Memory.ClassFactory.Create<Vehicle>(ReadUInt32(HuskOffset));

    internal const uint IsActiveOffset = HuskOffset + sizeof(uint);
    public bool IsActive
    {
        get => ReadBoolean(IsActiveOffset);
        set => WriteBoolean(IsActiveOffset, value);
    }

    internal const uint LaneOffset = IsActiveOffset + 4; // Padding
    //public Lane Lange => Memory.ClassFactory.Create<Lane>(ReadUInt32(LaneOffset));

    internal const uint HasHuskOffset = LaneOffset + sizeof(uint);
    public bool HasHusk
    {
        get => ReadBoolean(HasHuskOffset);
        set => WriteBoolean(HasHuskOffset, value);
    }

    public const uint Size = HasHuskOffset + 4; // Padding
}
