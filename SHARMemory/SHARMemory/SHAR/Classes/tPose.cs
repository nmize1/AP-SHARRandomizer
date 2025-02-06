using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVtPose@@")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical naming")]
public class tPose : Class
{
    public tPose(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    public int NumJoints
    {
        get => ReadInt32(8);
        set => WriteInt32(8, value);
    }

    public int NumRealJoints
    {
        get => ReadInt32(12);
        set => WriteInt32(12, value);
    }

    public tSkeleton Skeleton => Memory.ClassFactory.Create<tSkeleton>(16);

    public Joint Joint => Memory.ClassFactory.Create<Joint>(20);

    public bool PoseReady
    {
        get => ReadBoolean(24);
        set => WriteBoolean(24, value);
    }

    public uint UpdateCound
    {
        get => ReadUInt32(28);
        set => WriteUInt32(28, value);
    }
}
