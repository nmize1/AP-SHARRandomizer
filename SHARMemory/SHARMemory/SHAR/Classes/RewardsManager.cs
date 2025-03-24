using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVRewardsManager@@")]
public class RewardsManager : Class
{
    public const int MAX_INVENTORY = 8;
    public const int MAX_CAR_ATTRIBUTE_RECORDS = 50;

    public RewardsManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint LoadingManagerProcessRequestsCallbackVFTableOffset = 0;

    internal const uint RewardsListOffset = LoadingManagerProcessRequestsCallbackVFTableOffset + sizeof(uint);
    public StructArray<LevelRewardRecord> RewardsList => new(Memory, Address + RewardsListOffset, LevelRewardRecord.Size, 8);

    internal const uint CarAttributeListOffset = RewardsListOffset + LevelRewardRecord.Size * 8;
    public StructArray<CarAttributeRecord> CarAttributeList => new(Memory, Address + CarAttributeListOffset, CarAttributeRecord.Size, MAX_CAR_ATTRIBUTE_RECORDS);

    internal const uint CarAttributeRecordIndexOffset = CarAttributeListOffset + CarAttributeRecord.Size * MAX_CAR_ATTRIBUTE_RECORDS;
    public int CarAttributeRecordIndex
    {
        get => ReadInt32(CarAttributeRecordIndexOffset);
        set => WriteInt32(CarAttributeRecordIndexOffset, value);
    }

    internal const uint UpdateQueOffset = CarAttributeRecordIndexOffset + sizeof(int);
    public int UpdateQue
    {
        get => ReadInt32(UpdateQueOffset);
        set => WriteInt32(UpdateQueOffset, value);
    }

    internal const uint LevelTokenStoreListOffset = UpdateQueOffset + sizeof(int);
    public StructArray<TokenStoreInventory> LevelTokenStoreList => new(Memory, Address + LevelTokenStoreListOffset, TokenStoreInventory.Size, CharacterSheet.MAX_LEVELS);

    internal const uint LevelTokenStoreSearchIndexOffset = LevelTokenStoreListOffset + TokenStoreInventory.Size * CharacterSheet.MAX_LEVELS;
    public StructArray<int> LevelTokenStoreSearchIndex => new(Memory, Address + LevelTokenStoreSearchIndexOffset, sizeof(int), CharacterSheet.MAX_LEVELS);

    internal const uint ScriptLoadedOffset = LevelTokenStoreSearchIndexOffset + sizeof(int) * CharacterSheet.MAX_LEVELS;
    public bool ScriptLoaded
    {
        get => ReadBoolean(ScriptLoadedOffset);
        set => WriteBoolean(ScriptLoadedOffset, value);
    }
}
