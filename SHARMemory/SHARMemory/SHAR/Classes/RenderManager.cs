using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVRenderManager@@")]
public class RenderManager : ChunkListenerCallback
{
    public RenderManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint ChunkListenerCallbackVFTableOffset = 0;

    internal const uint LoadingManagerProcessRequestsCallbackVFTableOffset = ChunkListenerCallbackVFTableOffset + sizeof(uint);

    internal const uint EventListenerVFTableOffset = LoadingManagerProcessRequestsCallbackVFTableOffset + sizeof(uint);

    internal const uint EntityDeletionListOffset = EventListenerVFTableOffset + sizeof(uint);
    public PointerArray<tRefCounted> EntityDeletionList => PointerArrayExtensions.FromSwapArray<tRefCounted>(Memory, this, EntityDeletionListOffset);

    internal const uint RenderLayersOffset = EntityDeletionListOffset + 16;
    public PointerArray<RenderLayer> RenderLayers => new(Memory, Address + RenderLayersOffset, (int)Globals.RenderEnums.LayerEnum.numLayers);

    internal const uint CloudsOffset = RenderLayersOffset + sizeof(uint) * (int)Globals.RenderEnums.LayerEnum.numLayers;
    public tMultiController Clouds => Memory.ClassFactory.Create<tMultiController>(ReadUInt32(CloudsOffset));

    internal const uint LayerOffset = CloudsOffset + sizeof(uint);
    public int Layer
    {
        get => ReadInt32(LayerOffset);
        set => WriteInt32(LayerOffset, value);
    }

    internal const uint LevelOffset = LayerOffset + sizeof(int);
    public int Level
    {
        get => ReadInt32(LevelOffset);
        set => WriteInt32(LevelOffset, value);
    }

    internal const uint MissionOffset = LevelOffset + sizeof(int);
    public int Mission
    {
        get => ReadInt32(MissionOffset);
        set => WriteInt32(MissionOffset, value);
    }

    internal const uint CurWorldLayerOffset = MissionOffset + sizeof(int);
    public int CurWorldLayer
    {
        get => ReadInt32(CurWorldLayerOffset);
        set => WriteInt32(CurWorldLayerOffset, value);
    }

    internal const uint DoneInitialLoadOffset = CurWorldLayerOffset + sizeof(int);
    public bool DoneInitialLoad
    {
        get => ReadBoolean(DoneInitialLoadOffset);
        set => WriteBoolean(DoneInitialLoadOffset, value);
    }

    internal const uint ZELsOffset = DoneInitialLoadOffset + 4; // Padding
    public PointerArray<ZoneEventLocator> ZELs => PointerArrayExtensions.FromSwapArray<ZoneEventLocator>(Memory, this, ZELsOffset);

    internal const uint ZELOffset = ZELsOffset + 16;
    public ZoneEventLocator ZEL => Memory.ClassFactory.Create<ZoneEventLocator>(ReadUInt32(ZELOffset));

    internal const uint DynaLoadingOffset = ZELOffset + sizeof(uint);
    public bool DynaLoading
    {
        get => ReadBoolean(DynaLoadingOffset);
        set => WriteBoolean(DynaLoadingOffset, value);
    }

    internal const uint FirstDynamicZoneOffset = DynaLoadingOffset + sizeof(bool);
    public bool FirstDynamicZone
    {
        get => ReadBoolean(FirstDynamicZoneOffset);
        set => WriteBoolean(FirstDynamicZoneOffset, value);
    }

    internal const uint DrivingTooFastLoadOffset = FirstDynamicZoneOffset + sizeof(bool);
    public bool DrivingTooFastLoad
    {
        get => ReadBoolean(DrivingTooFastLoadOffset);
        set => WriteBoolean(DrivingTooFastLoadOffset, value);
    }

    internal const uint LoadZonesDumpedOffset = DrivingTooFastLoadOffset + sizeof(bool);
    public bool LoadZonesDumped
    {
        get => ReadBoolean(LoadZonesDumpedOffset);
        set => WriteBoolean(LoadZonesDumpedOffset, value);
    }

    internal const uint IgnoreVisibilityClearOffset = LoadZonesDumpedOffset + sizeof(bool);
    public bool IgnoreVisibilityClear
    {
        get => ReadBoolean(IgnoreVisibilityClearOffset);
        set => WriteBoolean(IgnoreVisibilityClearOffset, value);
    }

    internal const uint InVisibilityVolumeOffset = IgnoreVisibilityClearOffset + sizeof(bool);
    public bool InVisibilityVolume
    {
        get => ReadBoolean(InVisibilityVolumeOffset);
        set => WriteBoolean(InVisibilityVolumeOffset, value);
    }

    internal const uint AverageFrameTimeOffset = InVisibilityVolumeOffset + 3; // Padding
    public float AverageFrameTime
    {
        get => ReadSingle(AverageFrameTimeOffset);
        set => WriteSingle(AverageFrameTimeOffset, value);
    }

    internal const uint EnableMotionBlurOffset = AverageFrameTimeOffset + sizeof(float);
    public bool EnableMotionBlur
    {
        get => ReadBoolean(EnableMotionBlurOffset);
        set => WriteBoolean(EnableMotionBlurOffset, value);
    }

    internal const uint BlurAlphaOffset = EnableMotionBlurOffset + 4; // Padding
    public float BlurAlpha
    {
        get => ReadSingle(BlurAlphaOffset);
        set => WriteSingle(BlurAlphaOffset, value);
    }

    internal const uint MoodOffset = BlurAlphaOffset + sizeof(float);
    public MoodLighting Mood
    {
        get => ReadStruct<MoodLighting>(MoodOffset);
        set => WriteStruct(MoodOffset, value);
    }
}
