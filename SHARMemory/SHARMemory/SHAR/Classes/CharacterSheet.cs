using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using System.Text;

namespace SHARMemory.SHAR.Classes;

public class CharacterSheet : Class
{
    public const int MAX_LEVELS = 7;
    public const int MAX_MISSIONS = 8;
    public const int MAX_CARDS = 7;
    public const int MAX_STREETRACES = 3;
    public const int MAX_PURCHASED_ITEMS = 12;
    public const int MAX_LEVEL_GAGS = 32;
    public const int MAX_CARS_OWNED = 60;
    public const int NUM_BYTES_FOR_PERSISTENT_STATES = 1312;

    public enum PersistentObjectStateSector
    {
        L1R1,
        L1R2,
        L1R3,
        L1R4A,
        L1R4B,
        L1R6,
        L1R7,
        L1Z1,
        L1Z2,
        L1Z3,
        L1Z4,
        L1Z6,
        L1Z7,
        L2R1,
        L2R2,
        L2R3,
        L2R4,
        L2Z1,
        L2Z2,
        L2Z3,
        L2Z4,
        L3R1,
        L3R2,
        L3R3,
        L3R4,
        L3R5,
        L3Z1,
        L3Z2,
        L3Z3,
        L3Z4,
        L3Z5,
        L4R1,
        L4R2,
        L4R3,
        L4R4A,
        L4R4B,
        L4R6,
        L4R7,
        L4Z1,
        L4Z2,
        L4Z3,
        L4Z4,
        L4Z6,
        L4Z7,
        L5R1,
        L5R2,
        L5R3,
        L5R4,
        L5Z1,
        L5Z2,
        L5Z3,
        L5Z4,
        L6R1,
        L6R2,
        L6R3,
        L6R4,
        L6R5,
        L6Z1,
        L6Z2,
        L6Z3,
        L6Z4,
        L6Z5,
        L7R1,
        L7R2,
        L7R3,
        L7R4A,
        L7R4B,
        L7R6,
        L7R7,
        L7Z1,
        L7Z2,
        L7Z3,
        L7Z4,
        L7Z6,
        L7Z7,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
    }

    public CharacterSheet(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint PlayerNameOffset = 0;
    public string PlayerName => ReadString(PlayerNameOffset, Encoding.ASCII, 16u);

    internal const uint LevelListOffset = PlayerNameOffset + 16;
    public StructArray<LevelRecord> LevelList => new(Memory, Address + LevelListOffset, LevelRecord.Size, MAX_LEVELS);

    internal const uint CurrentMissionInfoOffset = LevelListOffset + LevelRecord.Size * MAX_LEVELS;
    public CurrentMission CurrentMissionInfo
    {
        get => ReadStruct<CurrentMission>(CurrentMissionInfoOffset);
        set => WriteStruct(CurrentMissionInfoOffset, value);
    }

    internal const uint HighestMissionPlayedOffset = CurrentMissionInfoOffset + CurrentMission.Size;
    public CurrentMission HighestMissionPlayed
    {
        get => ReadStruct<CurrentMission>(HighestMissionPlayedOffset);
        set => WriteStruct(HighestMissionPlayedOffset, value);
    }

    internal const uint IsNavSystemEnabledOffset = HighestMissionPlayedOffset + CurrentMission.Size;
    public bool IsNavSystemEnabled
    {
        get => ReadBoolean(IsNavSystemEnabledOffset);
        set => WriteBoolean(IsNavSystemEnabledOffset, value);
    }

    internal const uint CoinsOffset = IsNavSystemEnabledOffset + 4;
    public int Coins
    {
        get => ReadInt32(CoinsOffset);
        set => WriteInt32(CoinsOffset, value);
    }

    internal const uint CarInventoryOffset = CoinsOffset + sizeof(int);
    public CarInventory CarInventory
    {
        get => ReadStruct<CarInventory>(CarInventoryOffset);
        set => WriteStruct(CarInventoryOffset, value);
    }

    internal const uint PersistentObjectStatesOffset = CarInventoryOffset + CarInventory.Size;
    public StructArray<byte> PersistentObjectStates => new(Memory, Address + PersistentObjectStatesOffset, sizeof(byte), NUM_BYTES_FOR_PERSISTENT_STATES);

    internal const uint StateOffset = PersistentObjectStatesOffset + NUM_BYTES_FOR_PERSISTENT_STATES;
    //public byte State
    //{
    //    get => ReadByte(StateOffset);
    //    set => WriteByte(StateOffset, value);
    //}
    public bool ItchyScratchyCBGFirst
    {
        get => ReadBitfield(StateOffset, 0);
        set => WriteBitfield(StateOffset, 0, value);
    }

    public bool ItchyScratchyTicket
    {
        get => ReadBitfield(StateOffset, 1);
        set => WriteBitfield(StateOffset, 1, value);
    }

    public bool IsPersistentObjectDestroyed(PersistentObjectStateSector sector, int index)
    {
        var sectorInt = (int)sector;
        if (sectorInt < 0 || sectorInt > 81)
            throw new System.ArgumentOutOfRangeException(nameof(sector), "Invalid sector");

        if (index < 0 || index > 127)
            throw new System.ArgumentOutOfRangeException(nameof(index), "Invalid index");

        int sectorOffset = sectorInt * 16;
        int byteIndex = index / 8;
        int bitIndex = index % 8;

        return (PersistentObjectStates[sectorOffset + byteIndex] & (1 << bitIndex % 8)) == 0;
    }

    public void SetPersistentObjectDestroyed(PersistentObjectStateSector sector, int index, bool isDestroyed)
    {
        var sectorInt = (int)sector;
        if (sectorInt < 0 || sectorInt > 81)
            throw new System.ArgumentOutOfRangeException(nameof(sector), "Invalid sector");

        if (index < 0 || index > 127)
            throw new System.ArgumentOutOfRangeException(nameof(index), "Invalid index");

        int sectorOffset = sectorInt * 16;
        int byteIndex = index / 8;
        int bitIndex = index % 8;

        var currentValue = PersistentObjectStates[sectorOffset + byteIndex];
        if (isDestroyed)
            PersistentObjectStates[sectorOffset + byteIndex] = (byte)(currentValue & ~(1 << bitIndex)); // Clear bit (set to 0)
        else
            PersistentObjectStates[sectorOffset + byteIndex] = (byte)(currentValue | (1 << bitIndex));  // Set bit (set to 1)
    }
}
