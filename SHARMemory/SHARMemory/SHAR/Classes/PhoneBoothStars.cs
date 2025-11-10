using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

public class PhoneBoothStars : Class
{
    public const int MAX_NUM_STARS = 4;

    public PhoneBoothStars(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint StarsOffset = 0;
    public PointerArray<FeEntity> Stars => new(Memory, Address + StarsOffset, MAX_NUM_STARS);
}
