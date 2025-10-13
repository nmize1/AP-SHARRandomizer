using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Arrays;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVActorManager@@")]
public class ActorManager : EventListener
{
    public ActorManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ActorListOffset = EventListenerVFTableOffset + sizeof(uint);
    public PointerSwapArray<Actor> ActorList => new(Memory, Address + ActorListOffset);

    internal const uint SpawnPointListOffset = ActorListOffset + 16;

    internal const uint ActorBankOffset = SpawnPointListOffset + 16;
    public PointerSwapArray<Actor> ActorBank => new(Memory, Address + ActorBankOffset);

    internal const uint RemoveQueueOffset = ActorBankOffset + 16;
}
