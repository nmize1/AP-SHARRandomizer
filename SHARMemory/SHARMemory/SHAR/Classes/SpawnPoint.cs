using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Arrays;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVSpawnPoint@@")]
public class SpawnPoint : TriggerLocator
{
    public SpawnPoint(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint EventListenerVTTableOffset = PlayerIDOffset + sizeof(int);

    internal const uint SphereOffset = EventListenerVTTableOffset + sizeof(uint);
    public Sphere Sphere
    {
        get => ReadStruct<Sphere>(SphereOffset);
        set => WriteStruct(SphereOffset, value);
    }

    internal const uint SpawnTimeoutOffset = SphereOffset + Sphere.Size;
    public uint SpawnTimeout
    {
        get => ReadUInt32(SpawnTimeoutOffset);
        set => WriteUInt32(SpawnTimeoutOffset, value);
    }

    internal const uint TriggerVolumeOffset = SpawnTimeoutOffset + sizeof(uint);
    public TriggerVolume TriggerVolume => Memory.ClassFactory.Create<TriggerVolume>(ReadUInt32(TriggerVolumeOffset));

    internal const uint StatePropNameOffset = TriggerVolumeOffset + sizeof(uint);
    public long StatePropName
    {
        get => ReadInt64(StatePropNameOffset);
        set => WriteInt64(StatePropNameOffset, value);
    }

    internal const uint SpawnPointNameOffset = StatePropNameOffset + sizeof(long);
    public long SpawnPointName
    {
        get => ReadInt64(SpawnPointNameOffset);
        set => WriteInt64(SpawnPointNameOffset, value);
    }

    internal const uint TimeActorDestroyedOffset = SpawnPointNameOffset + sizeof(long);
    public uint TimeActorDestroyed
    {
        get => ReadUInt32(TimeActorDestroyedOffset);
        set => WriteUInt32(TimeActorDestroyedOffset, value);
    }

    internal const uint TimeOutPeriodOffset = TimeActorDestroyedOffset + sizeof(uint);
    public uint TimeOutPeriod
    {
        get => ReadUInt32(TimeOutPeriodOffset);
        set => WriteUInt32(TimeOutPeriodOffset, value);
    }

    internal const uint WasDestroyedOffset = TimeOutPeriodOffset + sizeof(uint);
    public bool WasDestroyed
    {
        get => ReadBoolean(WasDestroyedOffset);
        set => WriteBoolean(WasDestroyedOffset, value);
    }

    internal const uint PersistentObjectIDOffset = WasDestroyedOffset + 2; // Padding
    public ushort PersistentObjectID
    {
        get => ReadUInt16(PersistentObjectIDOffset);
        set => WriteUInt16(PersistentObjectIDOffset, value);
    }

    internal const uint BehavioursOffset = PersistentObjectIDOffset + sizeof(ushort);
    public PointerSwapArray<Behaviour> Behaviours => new(Memory, Address + BehavioursOffset);
}
