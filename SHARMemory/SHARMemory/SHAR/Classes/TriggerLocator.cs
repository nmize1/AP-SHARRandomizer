using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVTriggerLocator@@")]
public class TriggerLocator : Locator
{
    public TriggerLocator(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint TriggerVolumesOffset = LocationOffset + Vector3.Size;
    public PointerArray<TriggerVolume> TriggerVolumes => new(Memory, ReadUInt32(TriggerVolumesOffset), MaxNumTriggers);

    internal const uint NumTriggersOffset = TriggerVolumesOffset + sizeof(uint);
    public ushort NumTriggers
    {
        get => ReadUInt16(NumTriggersOffset);
        set => WriteUInt16(NumTriggersOffset, value);
    }

    internal const uint MaxNumTriggersOffset = NumTriggersOffset + sizeof(ushort);
    public ushort MaxNumTriggers
    {
        get => ReadUInt16(MaxNumTriggersOffset);
        set => WriteUInt16(MaxNumTriggersOffset, value);
    }

    internal const uint PlayerEnteredOffset = MaxNumTriggersOffset + sizeof(ushort);
    public bool PlayerEntered
    {
        get => ReadBoolean(PlayerEnteredOffset);
        set => WriteBoolean(PlayerEnteredOffset, value);
    }

    internal const uint PlayerIDOffset = PlayerEnteredOffset + 4; // Padding
    public int PlayerID
    {
        get => ReadInt32(PlayerIDOffset);
        set => WriteInt32(PlayerIDOffset, value);
    }
}
