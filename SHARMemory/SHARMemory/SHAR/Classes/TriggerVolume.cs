using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVTriggerVolume@@")]
public class TriggerVolume : IEntityDSG
{
    public TriggerVolume(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint LocatorOffset = SpatialNodeOffset + sizeof(uint);
    public TriggerLocator Locator => Memory.ClassFactory.Create<TriggerLocator>(ReadUInt32(LocatorOffset));

    internal const uint PositionOffset = LocatorOffset + sizeof(uint);
    public Vector3 Position
    {
        get => ReadStruct<Vector3>(PositionOffset);
        set => WriteStruct(PositionOffset, value);
    }

    internal const uint TrackingPlayersOffset = PositionOffset + Vector3.Size;
    public byte TrackingPlayers
    {
        get => ReadByte(TrackingPlayersOffset);
        set => WriteByte(TrackingPlayersOffset, value);
    }

    internal const uint FrameUsedOffset = TrackingPlayersOffset + 4; // Padding
    public uint FrameUsed
    {
        get => ReadUInt32(FrameUsedOffset);
        set => WriteUInt32(FrameUsedOffset, value);
    }

    internal const uint UserOffset = FrameUsedOffset + sizeof(uint);
    public int User
    {
        get => ReadInt32(UserOffset);
        set => WriteInt32(UserOffset, value);
    }
}
