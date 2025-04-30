using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVTrafficManager@@")]
public class TrafficManager : Class
{
    public const int MaxCharsToStopFor = 10;
    public const int VanillaMaxTraffic = 5;
    public const int MaxTrafficModelGroups = 10;
    public const int MaxIntersections = 5;
    public const int NumSwatchColours = 25;
    public const int MaxQueuedTrafficHorns = 3;

    private readonly int MaxTrafficValue;
    public TrafficManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator)
    {
        if (memory.ModLauncherOrdinalAddresses.TryGetValue(Memory.ModLauncherOrdinals.MaxTraffic, out uint MaxTrafficAddress))
        {
            MaxTrafficValue = memory.ReadInt32(MaxTrafficAddress);
        }
        else
        {
            MaxTrafficValue = VanillaMaxTraffic;
        }
    }

    public StructArray<SwatchColour> SwatchColours => new(Memory, Memory.SelectAddress(0x64A700, 0x64A6F0, 0x64A6F0, 0x64A700), SwatchColour.Size, NumSwatchColours);

    internal const uint DesiredTrafficSpeedKphOffset = sizeof(uint) + sizeof(uint); // ITrafficSpawnController VFTable + EventListener VFTable
    public float DesiredTrafficSpeedKph
    {
        get => ReadSingle(DesiredTrafficSpeedKphOffset);
        set => WriteSingle(DesiredTrafficSpeedKphOffset, value);
    }

    internal const uint MaxTrafficOffset = DesiredTrafficSpeedKphOffset + sizeof(float);
    public int MaxTraffic
    {
        get => ReadInt32(MaxTrafficOffset);
        set => WriteInt32(MaxTrafficOffset, value);
    }

    internal const uint NumTrafficOffset = MaxTrafficOffset + sizeof(int);
    public int NumTraffic
    {
        get => ReadInt32(NumTrafficOffset);
        set => WriteInt32(NumTrafficOffset, value);
    }

    internal const uint VehiclesOffset = NumTrafficOffset + sizeof(int);
    public ClassArray<TrafficVehicle> Vehicles => new(Memory, ReadUInt32(VehiclesOffset), TrafficVehicle.Size, MaxTrafficValue);

    internal const uint CharactersToStopForOffset = VehiclesOffset + sizeof(uint);
    public PointerArray<Character> CharactersToStopFor => new(Memory, Address + CharactersToStopForOffset, MaxCharsToStopFor);

    internal const uint NumCharsToStopForOffset = CharactersToStopForOffset + sizeof(uint) * MaxCharsToStopFor;
    public int NumCharsToStopFor
    {
        get => ReadInt32(NumCharsToStopForOffset);
        set => WriteInt32(NumCharsToStopForOffset, value);
    }

    internal const uint IntersectionsOffset = NumCharsToStopForOffset + sizeof(int);
    public PointerArray<SHARMemory.Memory.Class> Intersections => new(Memory, Address + IntersectionsOffset, MaxIntersections); // TODO: Add Intersection. Looks scary

    internal const uint MillisecondsBetweenRemoveOffset = IntersectionsOffset + sizeof(uint) * MaxIntersections;
    public uint MillisecondsBetweenRemove
    {
        get => ReadUInt32(MillisecondsBetweenRemoveOffset);
        set => WriteUInt32(MillisecondsBetweenRemoveOffset, value);
    }

    internal const uint MillisecondsBetweenAddOffset = MillisecondsBetweenRemoveOffset + sizeof(uint);
    public uint MillisecondsBetweenAdd
    {
        get => ReadUInt32(MillisecondsBetweenAddOffset);
        set => WriteUInt32(MillisecondsBetweenAddOffset, value);
    }

    internal const uint MillisecondsPopulateWorldOffset = MillisecondsBetweenAddOffset + sizeof(uint);
    public uint MillisecondsPopulateWorld
    {
        get => ReadUInt32(MillisecondsPopulateWorldOffset);
        set => WriteUInt32(MillisecondsPopulateWorldOffset, value);
    }

    internal const uint TrafficEnabledOffset = MillisecondsPopulateWorldOffset + sizeof(uint);
    public bool TrafficEnabled
    {
        get => ReadBoolean(TrafficEnabledOffset);
        set => WriteBoolean(TrafficEnabledOffset, value);
    }

    internal const uint TrafficModelGroupsOffset = TrafficEnabledOffset + 4; // Padding
    // TODO: This. Scary.

    internal const uint CurrTrafficModelGroupOffset = TrafficModelGroupsOffset + 3840; // TrafficModelGroup.Size * MaxTrafficModelGroups
    public int CurrTrafficModelGroup
    {
        get => ReadInt32(CurrTrafficModelGroupOffset);
        set => WriteInt32(CurrTrafficModelGroupOffset, value);
    }

    // TODO: QueuedTrafficHorns
}
