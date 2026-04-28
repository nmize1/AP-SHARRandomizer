using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVPlayerDrawable@@")]
public class PlayerDrawable : tDrawable
{
    public PlayerDrawable(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint PlayerOffset = NameOffset + sizeof(long);
    public AnimationPlayer Player => Memory.ClassFactory.Create<AnimationPlayer>(ReadUInt32(PlayerOffset));

    internal const uint RenderLayerOffset = PlayerOffset + sizeof(uint);
    public Globals.RenderEnums.LayerEnum RenderLayer
    {
        get => (Globals.RenderEnums.LayerEnum)ReadInt32(RenderLayerOffset);
        set => WriteInt32(RenderLayerOffset, (int)value);
    }
}
