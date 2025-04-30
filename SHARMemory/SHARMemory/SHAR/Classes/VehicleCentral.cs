using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVVehicleCentral@@")]
public class VehicleCentral : Class
{
    public VehicleCentral(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator)
    {
        if (memory.ModLauncherOrdinalAddresses.TryGetValue(Memory.ModLauncherOrdinals.MaxCars, out uint MaxVehiclesAddress) && memory.ModLauncherOrdinalAddresses.TryGetValue(Memory.ModLauncherOrdinals.CarsOffset, out uint ActiveVehiclesOffsetAddress))
        {
            MaxVehicles = memory.ReadInt32(MaxVehiclesAddress);
            ActiveVehiclesOffset = memory.ReadUInt32(ActiveVehiclesOffsetAddress);
        }
        else
        {
            MaxVehicles = 30;
            ActiveVehiclesOffset = 180;
        }
    }

    private readonly int MaxVehicles;
    private readonly uint ActiveVehiclesOffset;
    public PointerArray<Vehicle> ActiveVehicles => new(Memory, Address + ActiveVehiclesOffset, MaxVehicles);

}
