using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVtDrawablePose@@")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Radical naming")]
public class tDrawablePose : tDrawable
{
    public tDrawablePose(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint tPosableVFTableOffset = NameOffset + sizeof(long);

    internal const uint SkeletonOffset = tPosableVFTableOffset + sizeof(uint);
    public tSkeleton Skeleton => Memory.ClassFactory.Create<tSkeleton>(ReadUInt32(SkeletonOffset));

    internal const uint PoseOffset = SkeletonOffset + sizeof(uint);
    public tPose Pose => Memory.ClassFactory.Create<tPose>(ReadUInt32(PoseOffset));
}
