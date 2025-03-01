using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;

[ClassFactory.TypeInfoName(".?AVRoadManager@@")]
public class RoadManager : Class
{
    public RoadManager(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint RoadManagerVFTableOffset = 0;

    internal const uint RoadsOffset = RoadManagerVFTableOffset + sizeof(uint);
    public PointerArray<Road> Roads => new(Memory, ReadUInt32(RoadsOffset), (int)NumRoads);

    internal const uint NumRoadsOffset = RoadsOffset + sizeof(uint);
    public uint NumRoads
    {
        get => ReadUInt32(NumRoadsOffset);
        set => WriteUInt32(NumRoadsOffset, value);
    }

    internal const uint NumRoadsUsedOffset = NumRoadsOffset + sizeof(uint);
    public uint NumRoadsUsed
    {
        get => ReadUInt32(NumRoadsUsedOffset);
        set => WriteUInt32(NumRoadsUsedOffset, value);
    }

    internal const uint IntersectionsOffset = NumRoadsUsedOffset + sizeof(uint);
    public PointerArray<Intersection> Intersections => new(Memory, ReadUInt32(IntersectionsOffset), (int)NumIntersections);

    internal const uint NumIntersectionsOffset = IntersectionsOffset + sizeof(uint);
    public uint NumIntersections
    {
        get => ReadUInt32(NumIntersectionsOffset);
        set => WriteUInt32(NumIntersectionsOffset, value);
    }

    internal const uint NumIntersectionsUsedOffset = NumIntersectionsOffset + sizeof(uint);
    public uint NumIntersectionsUsed
    {
        get => ReadUInt32(NumIntersectionsUsedOffset);
        set => WriteUInt32(NumIntersectionsUsedOffset, value);
    }

    internal const uint BigIntersectionsOffset = NumIntersectionsUsedOffset + sizeof(uint);
    // SwapArray<BigIntersection*> mBigIntersections;

    internal const uint RoadSegmentDataOffset = BigIntersectionsOffset + 16;
    public PointerArray<RoadSegmentData> RoadSegmentData => new(Memory, ReadUInt32(Address), (int)NumRoadSegmentData);

    internal const uint NumRoadSegmentDataOffset = RoadSegmentDataOffset + sizeof(uint);
    public uint NumRoadSegmentData
    {
        get => ReadUInt32(NumRoadSegmentDataOffset);
        set => WriteUInt32(NumRoadSegmentDataOffset, value);
    }

    internal const uint NumRoadSegmentDataUsedOffset = NumRoadSegmentDataOffset + sizeof(uint);
    public uint NumRoadSegmentDataUsed
    {
        get => ReadUInt32(NumRoadSegmentDataUsedOffset);
        set => WriteUInt32(NumRoadSegmentDataUsedOffset, value);
    }

    internal const uint RoadSegmentsOffset = NumRoadSegmentDataUsedOffset + sizeof(uint);
    // RoadSegment** mRoadSegments;

    internal const uint NumRoadSegmentsOffset = RoadSegmentsOffset + sizeof(uint);
    public uint NumRoadSegments
    {
        get => ReadUInt32(NumRoadSegmentsOffset);
        set => WriteUInt32(NumRoadSegmentsOffset, value);
    }

    internal const uint NumRoadSegmentsUsedOffset = NumRoadSegmentsOffset + sizeof(uint);
    public uint NumRoadSegmentsUsed
    {
        get => ReadUInt32(NumRoadSegmentsUsedOffset);
        set => WriteUInt32(NumRoadSegmentsUsedOffset, value);
    }
}
