using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs.tMultiController;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVtMultiController@@")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical naming")]
public class tMultiController : tFrameController
{
    public tMultiController(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint DeltaTimeOffset = NameOffset + sizeof(long);
    public float DeltaTime
    {
        get => ReadSingle(DeltaTimeOffset);
        set => WriteSingle(DeltaTimeOffset, value);
    }

    internal const uint MinFrameOffset = DeltaTimeOffset + sizeof(float);
    public float MinFrame
    {
        get => ReadSingle(MinFrameOffset);
        set => WriteSingle(MinFrameOffset, value);
    }

    internal const uint MaxFrameOffset = MinFrameOffset + sizeof(float);
    public float MaxFrame
    {
        get => ReadSingle(MaxFrameOffset);
        set => WriteSingle(MaxFrameOffset, value);
    }

    internal const uint NumCyclesOffset = MaxFrameOffset + sizeof(float);
    public int NumCycles
    {
        get => ReadInt32(NumCyclesOffset);
        set => WriteInt32(NumCyclesOffset, value);
    }

    internal const uint TimeOffset = NumCyclesOffset + sizeof(int);
    public float Time
    {
        get => ReadSingle(TimeOffset);
        set => WriteSingle(TimeOffset, value);
    }

    internal const uint SpeedOffset = TimeOffset + sizeof(float);
    public float Speed
    {
        get => ReadSingle(SpeedOffset);
        set => WriteSingle(SpeedOffset, value);
    }

    internal const uint RelativeSpeedOffset = SpeedOffset + sizeof(float);
    public float RelativeSpeed
    {
        get => ReadSingle(RelativeSpeedOffset);
        set => WriteSingle(RelativeSpeedOffset, value);
    }

    internal const uint CycleModeOffset = RelativeSpeedOffset + sizeof(float);
    public P3DCycleMode CycleMode
    {
        get => (P3DCycleMode)ReadInt32(CycleModeOffset);
        set => WriteInt32(CycleModeOffset, (int)value);
    }

    internal const uint NumTracksOffset = CycleModeOffset + sizeof(int);
    public uint NumTracks
    {
        get => ReadUInt32(NumTracksOffset);
        set => WriteUInt32(NumTracksOffset, value);
    }

    internal const uint TrackInfoOffset = NumTracksOffset + sizeof(uint);
    public TrackInfo? TrackInfo
    {
        get
        {
            var address = ReadUInt32(TrackInfoOffset);
            if (address == 0)
                return null;

            return Memory.ReadStruct<TrackInfo>(address);
        }
    }

    internal const uint TracksOffset = TrackInfoOffset + sizeof(uint);
    public PointerArray<tFrameController> Tracks => new(Memory, ReadUInt32(TracksOffset), (int)NumTracks);
}
