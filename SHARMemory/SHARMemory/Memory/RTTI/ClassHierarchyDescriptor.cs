namespace SHARMemory.Memory.RTTI;

public class ClassHierarchyDescriptor : Class
{
    [System.Flags]
    public enum Attributes2
    {
        MultipleInheritance = 1,
        VirtualInheritance = 2,
    }

    public readonly uint Signature;
    public readonly Attributes2 Attributes;
    public readonly uint NumBaseClasses;
    public readonly BaseClassDescriptor[] BaseClassArray;

    public ClassHierarchyDescriptor(ProcessMemory memory, uint address, CompleteObjectLocator completeObjectLocator) : base(memory, address, completeObjectLocator)
    {
        memory.CheckValidMemoryAddress(address);
        Signature = ReadUInt32(0);
        Attributes = (Attributes2)ReadUInt32(4);
        NumBaseClasses = ReadUInt32(8);
        BaseClassArray = new PointerArray<BaseClassDescriptor>(Memory, ReadUInt32(12), (int)NumBaseClasses).ToArray();
    }
}