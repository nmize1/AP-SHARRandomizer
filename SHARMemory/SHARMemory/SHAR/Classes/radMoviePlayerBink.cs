using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVradMoviePlayerBink@@")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical's naming")]
public class radMoviePlayerBink : IRadMoviePlayer2
{
    public radMoviePlayerBink(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint LinkedClassNextOffset = IRefCountVFTableOffset + sizeof(uint);
    public radMoviePlayerBink LinkedClassNext => Memory.ClassFactory.Create<radMoviePlayerBink>(ReadUInt32(LinkedClassNextOffset));

    internal const uint LinkedClassPrevOffset = LinkedClassNextOffset + sizeof(uint);
    public radMoviePlayerBink LinkedClassPrev => Memory.ClassFactory.Create<radMoviePlayerBink>(ReadUInt32(LinkedClassPrevOffset));

    internal const uint radBaseObjectVFPtrOffset = LinkedClassPrevOffset + sizeof(uint);

    internal const uint ThisAllocatorOffset = radBaseObjectVFPtrOffset + sizeof(uint);
    public int ThisAllocator
    {
        get => ReadInt32(ThisAllocatorOffset);
        set => WriteInt32(ThisAllocatorOffset, value);
    }

    internal const uint RefCountOffset = ThisAllocatorOffset + sizeof(int);
    public int RefCount
    {
        get => ReadInt32(RefCountOffset);
        set => WriteInt32(RefCountOffset, value);
    }

    internal const uint RefIRadMovieRenderLoopOffset = RefCountOffset + sizeof(int);
    public SHARMemory.Memory.Class RefIRadMovieRenderLoop => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(RefIRadMovieRenderLoopOffset));

    internal const uint RefIRadMovieRenderStrategyOffset = RefIRadMovieRenderLoopOffset + sizeof(uint);
    public SHARMemory.Memory.Class RefIRadMovieRenderStrategy => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(RefIRadMovieRenderStrategyOffset));

    internal const uint StateOffset = RefIRadMovieRenderStrategyOffset + sizeof(uint);
    public States State
    {
        get => (States)ReadUInt32(StateOffset);
        set => WriteUInt32(StateOffset, (uint)value);
    }

    internal const uint BinkHandleOffset = StateOffset + sizeof(uint);
    public SHARMemory.Memory.Class BinkHandle => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(BinkHandleOffset));

    internal const uint VolumeOffset = BinkHandleOffset + sizeof(uint);
    public float Volume
    {
        get => ReadSingle(VolumeOffset);
        set => WriteSingle(VolumeOffset, value);
    }

    internal const uint PanOffset = VolumeOffset + sizeof(float);
    public float Pan
    {
        get => ReadSingle(PanOffset);
        set => WriteSingle(PanOffset, value);
    }

    internal const uint AudioTrackIndexOffset = PanOffset + sizeof(float);
    public uint AudioTrackIndex
    {
        get => ReadUInt32(AudioTrackIndexOffset);
        set => WriteUInt32(AudioTrackIndexOffset, value);
    }

    internal const uint IsPresentationOutstandingOffset = AudioTrackIndexOffset + sizeof(uint);
    public bool IsPresentationOutstanding
    {
        get => ReadBoolean(IsPresentationOutstandingOffset);
        set => WriteBoolean(IsPresentationOutstandingOffset, value);
    }

    internal const uint CheckAudioOffset = IsPresentationOutstandingOffset + sizeof(bool);
    public bool CheckAudio
    {
        get => ReadBoolean(CheckAudioOffset);
        set => WriteBoolean(CheckAudioOffset, value);
    }
}
