using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

public class Button : Class
{
    public Button(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public static uint TickCount(Memory memory) => memory.ReadUInt32(memory.SelectAddress(0x6C900C, 0, 0, 0));
    public uint TickCount() => TickCount(Memory);

    internal const uint ValueOffset = 0;
    public float Value
    {
        get => ReadSingle(ValueOffset);
        set => WriteSingle(ValueOffset, value);
    }

    internal const uint TickCountAtChangeOffset = ValueOffset + sizeof(float);
    public uint TickCountAtChange
    {
        get => ReadUInt32(TickCountAtChangeOffset);
        set => WriteUInt32(TickCountAtChangeOffset, value);
    }

    public const uint Size = TickCountAtChangeOffset + sizeof(uint);

    public void ForceChange()
    {
        TickCountAtChange = TickCount();
    }

    public void SetValue(float value)
    {
        Value = value;
        TickCountAtChange = TickCount();
    }
}
