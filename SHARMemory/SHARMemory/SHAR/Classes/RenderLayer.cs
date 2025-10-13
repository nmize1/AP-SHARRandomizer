using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVRenderLayer@@")]
public class RenderLayer : Class
{
    private const int MAX_PLAYERS = 4;

    public enum ExportedStates
    {
        Dead,
        Frozen,
        RenderReady
    }

    public RenderLayer(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint RenderLayerVTTableOffset = 0;

    internal const uint ExportedStateOffset = RenderLayerVTTableOffset + sizeof(uint);
    public ExportedStates ExportedState
    {
        get => (ExportedStates)ReadUInt32(ExportedStateOffset);
        set => WriteUInt32(ExportedStateOffset, (uint)value);
    }

    internal const uint PreviousStateOffset = ExportedStateOffset + sizeof(uint);
    public ExportedStates PreviousState
    {
        get => (ExportedStates)ReadUInt32(PreviousStateOffset);
        set => WriteUInt32(PreviousStateOffset, (uint)value);
    }

    internal const uint ViewOffset = PreviousStateOffset + sizeof(uint);
    //TODO: public PointerArray<tView> View => new(Memory, Address + ViewOffset, MAX_PLAYERS);

    internal const uint AlphaOffset = ViewOffset + sizeof(uint) * MAX_PLAYERS;
    public float Alpha
    {
        get => ReadSingle(AlphaOffset);
        set => WriteSingle(AlphaOffset, value);
    }

    internal const uint GutsOffset = AlphaOffset + sizeof(float);
    public PointerArray<tDrawable> Guts => PointerArrayExtensions.FromSwapArray<tDrawable>(Memory, this, GutsOffset);

    internal const uint IsBeginViewOffset = GutsOffset + 16;
    public bool IsBeginView
    {
        get => ReadBoolean(IsBeginViewOffset);
        set => WriteBoolean(IsBeginViewOffset, value);
    }

    internal const uint NumViewsOffset = IsBeginViewOffset + 4; // Padding
    public uint NumViews
    {
        get => ReadUInt32(NumViewsOffset);
        set => WriteUInt32(NumViewsOffset, value);
    }
}
