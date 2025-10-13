using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVCheatInputSystem@@")]
public class CheatInputSystem : Class
{
    public CheatInputSystem(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint CheatInputSystemVFTableOffset = 0;

    internal const uint EnabledOffset = CheatInputSystemVFTableOffset + sizeof(uint);
    public bool Enabled
    {
        get => ReadBoolean(EnabledOffset);
        set => WriteBoolean(EnabledOffset, value);
    }

    internal const uint ActivatedBitmaskOffset = EnabledOffset + 4; // Padding
    public uint ActivatedBitmask
    {
        get => ReadUInt32(ActivatedBitmaskOffset);
        set => WriteUInt32(ActivatedBitmaskOffset, value);
    }

    internal const uint CheatsDBOffset = ActivatedBitmaskOffset + sizeof(uint);
    public CheatsDB CheatsDB => Memory.ClassFactory.Create<CheatsDB>(ReadUInt32(CheatsDBOffset));

    internal const uint CheatInputHandlerOffset = CheatsDBOffset + sizeof(uint);
    //public CheatInputHandler CheatInputHandler => Memory.ClassFactory.Create<CheatInputHandler>(ReadUInt32(CheatInputHandler));

    //TODO
    //ICheatEnteredCallback* m_clientCallbacks[ MAX_NUM_CHEAT_CALLBACKS ];
    //int m_numClientCallbacks;
}
