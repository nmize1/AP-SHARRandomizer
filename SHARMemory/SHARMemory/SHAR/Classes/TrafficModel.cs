using SHARMemory.Memory.RTTI;

namespace SHARMemory.SHAR.Classes;
public class TrafficModel : Class
{
    public TrafficModel(Memory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator) { }

    internal const uint NumInstancesOffset = 0;
    public int NumInstances
    {
        get => ReadInt32(NumInstancesOffset);
        set => WriteInt32(NumInstancesOffset, value);
    }

    internal const uint ModelNameOffset = NumInstancesOffset + sizeof(int);
    public string ModelName
    {
        get => ReadString(ModelNameOffset, System.Text.Encoding.UTF8, 65);
        set => WriteString(ModelNameOffset, value, System.Text.Encoding.UTF8, 65);
    }

    internal const uint MaxInstancesOffset = ModelNameOffset + 65 + 3; // Padding
    public int MaxInstances
    {
        get => ReadInt32(MaxInstancesOffset);
        set => WriteInt32(MaxInstancesOffset, value);
    }

    internal const uint Size = MaxInstancesOffset + sizeof(int);
}
