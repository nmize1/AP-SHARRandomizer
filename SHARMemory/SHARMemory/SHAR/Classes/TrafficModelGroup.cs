using SHARMemory.Memory;
using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;
public class TrafficModelGroup : Class
{
    public const int MAX_TRAFFIC_MODELS = 5;

    public TrafficModelGroup(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint TrafficModelsOffset = 0;
    public ClassArray<TrafficModel> TrafficModels => new(Memory, Address + TrafficModelsOffset, TrafficModel.Size, MAX_TRAFFIC_MODELS);

    internal const uint NumModelsOffset = TrafficModelsOffset + TrafficModel.Size * MAX_TRAFFIC_MODELS;
    public int NumModels
    {
        get => ReadInt32(NumModelsOffset);
        set => WriteInt32(NumModelsOffset, value);
    }

    internal const uint Size = NumModelsOffset + sizeof(int);
}
