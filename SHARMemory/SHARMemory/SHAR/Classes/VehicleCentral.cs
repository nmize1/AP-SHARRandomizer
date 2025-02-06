using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVVehicleCentral@@")]
public class VehicleCentral : Class
{
    public VehicleCentral(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator)
    {
        if (memory.ModLauncherOrdinals.TryGetValue(3360, out uint MaxVehiclesAddress) && memory.ModLauncherOrdinals.TryGetValue(3364, out uint ActiveVehiclesOffsetAddress))
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
