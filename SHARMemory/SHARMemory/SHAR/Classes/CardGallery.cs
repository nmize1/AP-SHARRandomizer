using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCardGallery@@")]
public class CardGallery : Class
{
    public const int NUM_LEVELS = 7;
    public const int NUM_CARDS_PER_LEVEL = 7;

    public CardGallery(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint GameDataHandlerVFTableOffset = 0;
    internal const uint ICheatEnteredCallbackVFTableOffset = GameDataHandlerVFTableOffset + sizeof(uint);

    internal const uint CardsDBOffset = ICheatEnteredCallbackVFTableOffset + sizeof(uint);
    public CardsDB CardsDB => Memory.ClassFactory.Create<CardsDB>(ReadUInt32(CardsDBOffset));

    internal const uint CollectedCardsOffset = CardsDBOffset + sizeof(uint);
    public StructArray<CardList> CollectedCards => new(Memory, Address + CollectedCardsOffset, CardList.Size, 7);
}
