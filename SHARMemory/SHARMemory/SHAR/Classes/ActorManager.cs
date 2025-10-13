using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using System;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVActorManager@@")]
public class ActorManager : EventListener
{
    public ActorManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ActorListOffset = EventListenerVFTableOffset + sizeof(uint);
    public PointerArray<Actor> ActorList => PointerArrayExtensions.FromSwapArray<Actor>(Memory, this, ActorListOffset);

    internal const uint SpawnPointListOffset = ActorListOffset + 16;

    internal const uint ActorBankOffset = SpawnPointListOffset + 16;
    public PointerArray<Actor> ActorBank => PointerArrayExtensions.FromSwapArray<Actor>(Memory, this, ActorBankOffset);

    internal const uint RemoveQueueOffset = ActorBankOffset + 16;
}
