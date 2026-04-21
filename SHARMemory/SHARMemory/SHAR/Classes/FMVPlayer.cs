using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVFMVPlayer@@")]
public class FMVPlayer : AnimationPlayer
{
    public FMVPlayer(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint IRefCountVFPtrOffset1 = LoadDataCallbackOffset + sizeof(uint);

    internal const uint IRefCountVFPtrOffset2 = IRefCountVFPtrOffset1 + sizeof(uint);

    internal const uint radBaseObjectVFPtrOffset = IRefCountVFPtrOffset2 + sizeof(uint);

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

    internal const uint UserInputHandlerOffset = RefCountOffset + sizeof(int);
    public SHARMemory.Memory.Class UserInputHandler => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(UserInputHandlerOffset));

    internal const uint RefIRadMoviePlayerOffset = UserInputHandlerOffset + sizeof(uint);
    public IRadMoviePlayer2 RefIRadMoviePlayer => Memory.ClassFactory.Create<IRadMoviePlayer2>(ReadUInt32(RefIRadMoviePlayerOffset));
    //TODO ref<IRadMoviePlayer2>

    internal const uint FrameReadyOffset = RefIRadMoviePlayerOffset + sizeof(uint);
    public bool FrameReady
    {
        get => ReadBoolean(FrameReadyOffset);
        set => WriteBoolean(FrameReadyOffset, value);
    }

    internal const uint ElapsedTimeOffset = FrameReadyOffset + 4; //Padding
    public float ElapsedTime
    {
        get => ReadSingle(ElapsedTimeOffset);
        set => WriteSingle(ElapsedTimeOffset, value);
    }

    internal const uint DriveFinishedOffset = ElapsedTimeOffset + sizeof(float);
    public bool DriveFinished
    {
        get => ReadBoolean(DriveFinishedOffset);
        set => WriteBoolean(DriveFinishedOffset, value);
    }

    internal const uint FadeOutOffset = DriveFinishedOffset + 4; // Padding
    public float FadeOut
    {
        get => ReadSingle(FadeOutOffset);
        set => WriteSingle(FadeOutOffset, value);
    }

    internal const uint MovieVolumeOffset = FadeOutOffset + sizeof(float);
    public float MovieVolume
    {
        get => ReadSingle(MovieVolumeOffset);
        set => WriteSingle(MovieVolumeOffset, value);
    }
}
