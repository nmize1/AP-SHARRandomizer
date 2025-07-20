using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCardsDB@@")]
public class CardsDB : Class
{
    public const int MAX_NUM_CARDS = 64;

    public CardsDB(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint CardDBVFTableOffset = 0;

    internal const uint CardsOffset = CardDBVFTableOffset + sizeof(uint);
    public PointerArray<Card> Cards => new(Memory, ReadUInt32(CardsOffset), MAX_NUM_CARDS);

    internal const uint NumCardsOffset = CardsOffset + sizeof(uint);
    public int NumCards
    {
        get => ReadInt32(NumCardsOffset);
        set => WriteInt32(NumCardsOffset, value);
    }
}
