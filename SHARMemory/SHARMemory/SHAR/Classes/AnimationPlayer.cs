using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVAnimationPlayer@@")]
public class AnimationPlayer : Class
{
    public enum AnimState
    {
        Idle,
        Loading,
        Loaded,
        Playing,
        Stopped,
        NumStates
    }

    public AnimationPlayer(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint LoadingManagerProcessRequestsCallbackVFPtrOffset = 0;

    internal const uint StateOffset = LoadingManagerProcessRequestsCallbackVFPtrOffset + sizeof(uint);
    public AnimState State
    {
        get => (AnimState)ReadUInt32(StateOffset);
        set => WriteUInt32(StateOffset, (uint)value);
    }

    internal const uint PlayAfterLoadOffset = StateOffset + sizeof(uint);
    public bool PlayAfterLoad
    {
        get => ReadBitfield(PlayAfterLoadOffset, 0);
        set => WriteBitfield(PlayAfterLoadOffset, 0, value);
    }

    internal const uint ExclusiveOffset = PlayAfterLoadOffset + 0;
    public bool Exclusive
    {
        get => ReadBitfield(ExclusiveOffset, 1);
        set => WriteBitfield(ExclusiveOffset, 1, value);
    }

    internal const uint ShowAlwaysOffset = ExclusiveOffset + 0;
    public bool ShowAlways
    {
        get => ReadBitfield(ShowAlwaysOffset, 2);
        set => WriteBitfield(ShowAlwaysOffset, 2, value);
    }

    internal const uint KeepLayersFrozenOffset = ShowAlwaysOffset + 0;
    public bool KeepLayersFrozen
    {
        get => ReadBitfield(KeepLayersFrozenOffset, 3);
        set => WriteBitfield(KeepLayersFrozenOffset, 3, value);
    }

    internal const uint IsSkippableOffset = KeepLayersFrozenOffset + 0;
    public bool IsSkippable
    {
        get => ReadBitfield(IsSkippableOffset, 4);
        set => WriteBitfield(IsSkippableOffset, 4, value);
    }

    internal const uint SectionOffset = IsSkippableOffset + 4; // Padding
    public ulong Section
    {
        get => ReadUInt64(SectionOffset);
        set => WriteUInt64(SectionOffset, value);
    }

    internal const uint LoadDataCallbackOffset = SectionOffset + sizeof(ulong);
}
