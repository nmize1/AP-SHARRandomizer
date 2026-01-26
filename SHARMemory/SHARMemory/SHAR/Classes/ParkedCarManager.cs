using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVParkedCarManager@@")]
public class ParkedCarManager : EventListener
{
    public const int MAX_LOCATORS_PER_ZONE = 50;

    public ParkedCarManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint LoadingManagerProcessRequestsCallbackVFTableOffset = EventListenerVFTableOffset + sizeof(uint);

    internal const uint ParkedCarsOffset = LoadingManagerProcessRequestsCallbackVFTableOffset + sizeof(uint);
    public StructArray<ParkedCarInfo> ParkedCars => new(Memory, ReadUInt32(ParkedCarsOffset), ParkedCarInfo.Size, (int)NumParkedCars);

    internal const uint NumCarTypesOffset = ParkedCarsOffset + sizeof(uint);
    public uint NumCarTypes
    {
        get => ReadUInt32(NumCarTypesOffset);
        set => WriteUInt32(NumCarTypesOffset, value);
    }

    internal const uint NumParkedCarsOffset = NumCarTypesOffset + sizeof(uint);
    public uint NumParkedCars
    {
        get => ReadUInt32(NumParkedCarsOffset);
        set => WriteUInt32(NumParkedCarsOffset, value);
    }

    internal const uint LocatorsOffset = NumParkedCarsOffset + sizeof(uint);
    public PointerArray<CarStartLocator> Locators => new(Memory, Address + LocatorsOffset, (int)NumLocators);

    internal const uint NumLocatorsOffset = LocatorsOffset + sizeof(uint) * MAX_LOCATORS_PER_ZONE;
    public uint NumLocators
    {
        get => ReadUInt32(NumLocatorsOffset);
        set => WriteUInt32(NumLocatorsOffset, value);
    }

    internal const uint LoadingZoneUIDOffset = NumLocatorsOffset + sizeof(uint);
    public long LoadingZoneUID
    {
        get => ReadInt64(LoadingZoneUIDOffset);
        set => WriteInt64(LoadingZoneUIDOffset, value);
    }

    internal const uint FreeCarOffset = LoadingZoneUIDOffset + sizeof(long);
    public ParkedCarInfo FreeCar
    {
        get => ReadStruct<ParkedCarInfo>(FreeCarOffset);
        set => WriteStruct(FreeCarOffset, value);
    }

    internal const uint FreeCarLocatorOffset = FreeCarOffset + ParkedCarInfo.Size;
    public CarStartLocator FreeCarLocator => Memory.ClassFactory.Create<CarStartLocator>(ReadUInt32(FreeCarLocatorOffset));

    internal const uint ParkedCarsEnabledOffset = FreeCarLocatorOffset + sizeof(uint);
    public bool ParkedCarsEnabled
    {
        get => ReadBoolean(ParkedCarsEnabledOffset);
        set => WriteBoolean(ParkedCarsEnabledOffset, value);
    }
}
