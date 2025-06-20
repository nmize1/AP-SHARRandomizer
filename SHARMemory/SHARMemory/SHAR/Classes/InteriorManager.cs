using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;
using SHARMemory.SHAR.Structs.InteriorManager;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVInteriorManager@@")]
public class InteriorManager : Class
{
    public const int MAX_BINDINGS = 64;
    public const int MAX_GAGS = 32;

    public enum InteriorStates : uint
    {
        None,
        Enter,
        Exit,
        Inside
    }

    public InteriorManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint InteriorManagerVFTableOffset = 0;

    internal const uint InteriorStateOffset = InteriorManagerVFTableOffset + sizeof(uint);
    public InteriorStates InteriorState
    {
        get => (InteriorStates)ReadUInt32(InteriorStateOffset);
        set => WriteUInt32(InteriorStateOffset, (uint)value);
    }

    internal const uint EntryRequestedOffset = InteriorStateOffset + sizeof(uint);
    public bool EntryRequested
    {
        get => ReadBoolean(EntryRequestedOffset);
        set => WriteBoolean(EntryRequestedOffset, value);
    }

    internal const uint InteriorLoadedOffset = EntryRequestedOffset + 1;
    public bool InteriorLoaded
    {
        get => ReadBoolean(InteriorLoadedOffset);
        set => WriteBoolean(InteriorLoadedOffset, value);
    }

    internal const uint LoadedGagsOffset = InteriorLoadedOffset + 1;
    public bool LoadedGags
    {
        get => ReadBoolean(LoadedGagsOffset);
        set => WriteBoolean(LoadedGagsOffset, value);
    }

    internal const uint LoadedInteriorUIDOffset = LoadedGagsOffset + 2; // Padding
    public ulong LoadedInteriorUID
    {
        get => ReadUInt64(LoadedInteriorUIDOffset);
        set => WriteUInt64(LoadedInteriorUIDOffset, value);
    }

    internal const uint CurrentInteriorUIDOffset = LoadedInteriorUIDOffset + sizeof(ulong);
    public ulong CurrentInteriorUID
    {
        get => ReadUInt64(CurrentInteriorUIDOffset);
        set => WriteUInt64(CurrentInteriorUIDOffset, value);
    }

    internal const uint SectionOffset = CurrentInteriorUIDOffset + sizeof(ulong);
    public ulong Section
    {
        get => ReadUInt64(SectionOffset);
        set => WriteUInt64(SectionOffset, value);
    }

    internal const uint CollectionEffectOffset = SectionOffset + sizeof(ulong);
    public AnimatedIcon CollectionEffect => Memory.ClassFactory.Create<AnimatedIcon>(ReadUInt32(CollectionEffectOffset));

    internal const uint InOffset = CollectionEffectOffset + sizeof(uint);
    public bool In
    {
        get => ReadBoolean(InOffset);
        set => WriteBoolean(InOffset, value);
    }

    internal const uint ExitPosOffset = InOffset + 4; // Padding
    public Vector3 ExitPos
    {
        get => ReadStruct<Vector3>(ExitPosOffset);
        set => WriteStruct(ExitPosOffset, value);
    }

    internal const uint ExitFacingOffset = ExitPosOffset + Vector3.Size;
    public float ExitFacing
    {
        get => ReadSingle(ExitFacingOffset);
        set => WriteSingle(ExitFacingOffset, value);
    }

    internal const uint GagBindingsOffset = ExitFacingOffset + sizeof(float);
    public StructArray<GagBinding> GagBindings => new(Memory, Address + GagBindingsOffset, GagBinding.Size, MAX_BINDINGS);

    internal const uint BindingCountOffset = GagBindingsOffset + GagBinding.Size * MAX_BINDINGS;
    public int BindingCount
    {
        get => ReadInt32(BindingCountOffset);
        set => WriteInt32(BindingCountOffset, value);
    }

    internal const uint BuildGagOffset = BindingCountOffset + sizeof(int);
    public GagBinding BuildGag
    {
        get => ReadStruct<GagBinding>(BuildGagOffset);
        set => WriteStruct(BuildGagOffset, value);
    }

    internal const uint GagCountOffset = BuildGagOffset + GagBinding.Size;
    public int GagCount
    {
        get => ReadInt32(GagCountOffset);
        set => WriteInt32(GagCountOffset, value);
    }

    internal const uint GagsOffset = GagCountOffset + sizeof(int);
    public PointerArray<Gag> Gags => new(Memory, Address + GagsOffset, MAX_GAGS);

    internal const uint BuildingGagOffset = GagsOffset + sizeof(uint) * MAX_GAGS;
    public bool BuildingGag
    {
        get => ReadBoolean(BuildingGagOffset);
        set => WriteBoolean(BuildingGagOffset, value);
    }

    internal const uint ExitOffset = BuildingGagOffset + 4; // Padding
    public InteriorExit Exit => Memory.ClassFactory.Create<InteriorExit>(ReadUInt32(ExitOffset));

    internal const uint InteriorAnimationsOffset = ExitOffset + sizeof(uint);
    public tFrameController InteriorAnimations => Memory.ClassFactory.Create<tFrameController>(ReadUInt32(InteriorAnimationsOffset));

    internal const uint IsPlayingISDialogOffset = InteriorAnimationsOffset + sizeof(uint);
    public bool IsPlayingISDialog
    {
        get => ReadBoolean(IsPlayingISDialogOffset);
        set => WriteBoolean(IsPlayingISDialogOffset, value);
    }
}
