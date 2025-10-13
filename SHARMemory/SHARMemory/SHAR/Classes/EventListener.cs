using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVEventListener@@")]
public class EventListener : Class
{
    public EventListener(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint EventListenerVFTableOffset = 0;
}
