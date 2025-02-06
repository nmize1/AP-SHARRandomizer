using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVIEntityDSG@@")]
public class IEntityDSG : tDrawable
{
    public IEntityDSG(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint DrawableShaderCallbackVFTableOffset = NameOffset + sizeof(long);

    internal const uint RankOffset = DrawableShaderCallbackVFTableOffset + sizeof(uint);
    public float Rank
    {
        get => ReadSingle(RankOffset);
        set => WriteSingle(RankOffset, value);
    }

    internal const uint TranslucentOffset = RankOffset + sizeof(float);
    public bool Translucent
    {
        get => ReadBoolean(4);
        set => WriteBoolean(4, value);
    }

    internal const uint ShaderNameOffset = TranslucentOffset + 4; // Padding
    public long ShaderName
    {
        get => ReadInt64(ShaderNameOffset);
        set => WriteInt64(ShaderNameOffset, value);
    }

    internal const uint SpatialNodeOffset = ShaderNameOffset + sizeof(long);
    public SHARMemory.Memory.Class SpatialNode => Memory.ClassFactory.Create<SHARMemory.Memory.Class>(ReadUInt32(SpatialNodeOffset));
}
