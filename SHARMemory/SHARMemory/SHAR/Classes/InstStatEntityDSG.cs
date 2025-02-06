using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;
using SHARMemory.SHAR.Structs;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVInstStatEntityDSG@@")]
public class InstStatEntityDSG : StaticEntityDSG
{
    public InstStatEntityDSG(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint MatrixOffset = DrawstuffOffset + sizeof(uint);
    public Matrix4x4? Matrix
    {
        get
        {
            var address = ReadUInt32(MatrixOffset);
            if (address == 0)
                return null;

            return Memory.ReadStruct<Matrix4x4>(address);
        }
    }

    internal const uint ShadowDrawableOffset = MatrixOffset + sizeof(uint);
    public tDrawable ShadowDrawable => Memory.ClassFactory.Create<tDrawable>(ReadUInt32(ShadowDrawableOffset));

    internal const uint ShadowMatrixOffset = ShadowDrawableOffset + sizeof(uint);
    public Matrix4x4? ShadowMatrix
    {
        get
        {
            var address = ReadUInt32(ShadowMatrixOffset);
            if (address == 0)
                return null;

            return Memory.ReadStruct<Matrix4x4>(address);
        }
    }
}
